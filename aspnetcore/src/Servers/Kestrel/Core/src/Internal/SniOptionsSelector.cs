// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal
{
    internal class SniOptionsSelector
    {
        private const string WildcardHost = "*";
        private const string WildcardPrefix = "*.";

        private readonly string _endpointName;

        private readonly Func<ConnectionContext, string?, X509Certificate2?>? _fallbackServerCertificateSelector;
        private readonly Action<ConnectionContext, SslServerAuthenticationOptions>? _onAuthenticateCallback;

        private readonly Dictionary<string, SniOptions> _exactNameOptions = new Dictionary<string, SniOptions>(StringComparer.OrdinalIgnoreCase);
        private readonly SortedList<string, SniOptions> _wildcardPrefixOptions = new SortedList<string, SniOptions>(LongestStringFirstComparer.Instance);
        private readonly SniOptions? _wildcardOptions;

        public SniOptionsSelector(
            string endpointName,
            Dictionary<string, SniConfig> sniDictionary,
            ICertificateConfigLoader certifcateConfigLoader,
            HttpsConnectionAdapterOptions fallbackHttpsOptions,
            HttpProtocols fallbackHttpProtocols,
            ILogger<HttpsConnectionMiddleware> logger)
        {
            _endpointName = endpointName;

            _fallbackServerCertificateSelector = fallbackHttpsOptions.ServerCertificateSelector;
            _onAuthenticateCallback = fallbackHttpsOptions.OnAuthenticate;

            foreach (var (name, sniConfig) in sniDictionary)
            {
                var sslOptions = new SslServerAuthenticationOptions
                {
                    ServerCertificate = certifcateConfigLoader.LoadCertificate(sniConfig.Certificate, $"{endpointName}:Sni:{name}"),
                    EnabledSslProtocols = sniConfig.SslProtocols ?? fallbackHttpsOptions.SslProtocols,
                    CertificateRevocationCheckMode = fallbackHttpsOptions.CheckCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck,
                };

                if (sslOptions.ServerCertificate is null)
                {
                    if (fallbackHttpsOptions.ServerCertificate is null && _fallbackServerCertificateSelector is null)
                    {
                        throw new InvalidOperationException(CoreStrings.NoCertSpecifiedNoDevelopmentCertificateFound);
                    }

                    if (_fallbackServerCertificateSelector is null)
                    {
                        // Cache the fallback ServerCertificate since there's no fallback ServerCertificateSelector taking precedence. 
                        sslOptions.ServerCertificate = fallbackHttpsOptions.ServerCertificate;
                    }
                }

                if (sslOptions.ServerCertificate != null)
                {
                    // This might be do blocking IO but it'll resolve the certificate chain up front before any connections are
                    // made to the server
                    sslOptions.ServerCertificateContext = SslStreamCertificateContext.Create((X509Certificate2)sslOptions.ServerCertificate, additionalCertificates: null);
                }

                if (!certifcateConfigLoader.IsTestMock && sslOptions.ServerCertificate is X509Certificate2 cert2)
                {
                    HttpsConnectionMiddleware.EnsureCertificateIsAllowedForServerAuth(cert2);
                }

                var clientCertificateMode = sniConfig.ClientCertificateMode ?? fallbackHttpsOptions.ClientCertificateMode;

                if (clientCertificateMode != ClientCertificateMode.NoCertificate)
                {
                    sslOptions.ClientCertificateRequired = true;
                    sslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        HttpsConnectionMiddleware.RemoteCertificateValidationCallback(
                            clientCertificateMode, fallbackHttpsOptions.ClientCertificateValidation, certificate, chain, sslPolicyErrors);
                }

                var httpProtocols = sniConfig.Protocols ?? fallbackHttpProtocols;
                httpProtocols = HttpsConnectionMiddleware.ValidateAndNormalizeHttpProtocols(httpProtocols, logger);
                HttpsConnectionMiddleware.ConfigureAlpn(sslOptions, httpProtocols);

                var sniOptions = new SniOptions(sslOptions, httpProtocols);

                if (name.Equals(WildcardHost, StringComparison.Ordinal))
                {
                    _wildcardOptions = sniOptions;
                }
                else if (name.StartsWith(WildcardPrefix, StringComparison.Ordinal))
                {
                    // Only slice off 1 character, the `*`. We want to match the leading `.` also.
                    _wildcardPrefixOptions.Add(name.Substring(1), sniOptions);
                }
                else
                {
                    _exactNameOptions.Add(name, sniOptions);
                }
            }
        }

        public SslServerAuthenticationOptions GetOptions(ConnectionContext connection, string serverName)
        {
            SniOptions? sniOptions = null;

            if (!string.IsNullOrEmpty(serverName) && !_exactNameOptions.TryGetValue(serverName, out sniOptions))
            {
                foreach (var (suffix, options) in _wildcardPrefixOptions)
                {
                    if (serverName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        sniOptions = options;
                        break;
                    }
                }
            }

            // Fully wildcarded ("*") options can be used even when given an empty server name.
            sniOptions ??= _wildcardOptions;

            if (sniOptions is null)
            {
                if (serverName is null)
                {
                    // There was no ALPN
                    throw new AuthenticationException(CoreStrings.FormatSniNotConfiguredToAllowNoServerName(_endpointName));
                }
                else
                {
                    throw new AuthenticationException(CoreStrings.FormatSniNotConfiguredForServerName(serverName, _endpointName));
                }
            }

            connection.Features.Set(new HttpProtocolsFeature(sniOptions.HttpProtocols));

            var sslOptions = sniOptions.SslOptions;

            if (sslOptions.ServerCertificate is null)
            {
                Debug.Assert(_fallbackServerCertificateSelector != null,
                    "The cached SniOptions ServerCertificate can only be null if there's a fallback certificate selector.");

                // If a ServerCertificateSelector doesn't return a cert, HttpsConnectionMiddleware doesn't fallback to the ServerCertificate.
                sslOptions = CloneSslOptions(sslOptions);
                var fallbackCertificate = _fallbackServerCertificateSelector(connection, serverName);

                if (fallbackCertificate != null)
                {
                    HttpsConnectionMiddleware.EnsureCertificateIsAllowedForServerAuth(fallbackCertificate);
                }

                sslOptions.ServerCertificate = fallbackCertificate;
            }

            if (_onAuthenticateCallback != null)
            {
                // From doc comments: "This is called after all of the other settings have already been applied."
                sslOptions = CloneSslOptions(sslOptions);
                _onAuthenticateCallback(connection, sslOptions);
            }

            return sslOptions;
        }

        public static ValueTask<SslServerAuthenticationOptions> OptionsCallback(ConnectionContext connection, SslStream stream, SslClientHelloInfo clientHelloInfo, object state, CancellationToken cancellationToken)
        {
            var sniOptionsSelector = (SniOptionsSelector)state;
            var options = sniOptionsSelector.GetOptions(connection, clientHelloInfo.ServerName);
            return new ValueTask<SslServerAuthenticationOptions>(options);
        }

        internal static SslServerAuthenticationOptions CloneSslOptions(SslServerAuthenticationOptions sslOptions) =>
            new SslServerAuthenticationOptions
            {
                AllowRenegotiation = sslOptions.AllowRenegotiation,
                ApplicationProtocols = sslOptions.ApplicationProtocols?.ToList(),
                CertificateRevocationCheckMode = sslOptions.CertificateRevocationCheckMode,
                CipherSuitesPolicy = sslOptions.CipherSuitesPolicy,
                ClientCertificateRequired = sslOptions.ClientCertificateRequired,
                EnabledSslProtocols = sslOptions.EnabledSslProtocols,
                EncryptionPolicy = sslOptions.EncryptionPolicy,
                RemoteCertificateValidationCallback = sslOptions.RemoteCertificateValidationCallback,
                ServerCertificate = sslOptions.ServerCertificate,
                ServerCertificateContext = sslOptions.ServerCertificateContext,
                ServerCertificateSelectionCallback = sslOptions.ServerCertificateSelectionCallback,
            };

        private class SniOptions
        {
            public SniOptions(SslServerAuthenticationOptions sslOptions, HttpProtocols httpProtocols)
            {
                SslOptions = sslOptions;
                HttpProtocols = httpProtocols;
            }

            public SslServerAuthenticationOptions SslOptions { get; }
            public HttpProtocols HttpProtocols { get; }
        }

        private class LongestStringFirstComparer : IComparer<string>
        {
            public static LongestStringFirstComparer Instance { get; } = new LongestStringFirstComparer();

            private LongestStringFirstComparer()
            {
            }

            public int Compare(string? x, string? y)
            {
                // Flip x and y to put the longest instead of the shortest string first in the SortedList.
                return y!.Length.CompareTo(x!.Length);
            }
        }
    }
}
