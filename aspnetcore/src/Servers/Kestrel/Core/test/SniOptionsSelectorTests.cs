// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class SniOptionsSelectorTests
    {
        private static X509Certificate2 _x509Certificate2 = TestResources.GetTestCertificate();

        [Fact]
        public void PrefersExactMatchOverWildcardPrefixOverWildcardOnly()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "Exact"
                        }
                    }
                },
                {
                    "*.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "WildcardPrefix"
                        }
                    }
                },
                {
                    "*",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "WildcardOnly"
                        }
                    }
                }
            };

            var mockCertificateConfigLoader = new MockCertificateConfigLoader();
            var pathDictionary = mockCertificateConfigLoader.CertToPathDictionary;

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                mockCertificateConfigLoader,
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var wwwSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Equal("Exact", pathDictionary[wwwSubdomainOptions.ServerCertificate]);

            var baSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "b.a.example.org");
            Assert.Equal("WildcardPrefix", pathDictionary[baSubdomainOptions.ServerCertificate]);

            var aSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "a.example.org");
            Assert.Equal("WildcardPrefix", pathDictionary[aSubdomainOptions.ServerCertificate]);

            // "*.example.org" is preferred over "*", but "*.example.org" doesn't match "example.org".
            // REVIEW: Are we OK with "example.org" matching "*" instead of "*.example.org"? It feels annoying to me to have to configure example.org twice.
            // Unfortunately, the alternative would have "a.example.org" match "*.a.example.org" before "*.example.org", and that just seems wrong.
            var noSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "example.org");
            Assert.Equal("WildcardOnly", pathDictionary[noSubdomainOptions.ServerCertificate]);

            var anotherTldOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "dot.net");
            Assert.Equal("WildcardOnly", pathDictionary[anotherTldOptions.ServerCertificate]);
        }

        [Fact]
        public void PerfersLongerWildcardPrefixOverShorterWildcardPrefix()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "*.a.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "Long"
                        }
                    }
                },
                {
                    "*.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "Short"
                        }
                    }
                }
            };

            var mockCertificateConfigLoader = new MockCertificateConfigLoader();
            var pathDictionary = mockCertificateConfigLoader.CertToPathDictionary;

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                mockCertificateConfigLoader,
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var baSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "b.a.example.org");
            Assert.Equal("Long", pathDictionary[baSubdomainOptions.ServerCertificate]);

            // "*.a.example.org" is preferred over "*.example.org", but "a.example.org" doesn't match "*.a.example.org".
            var aSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "a.example.org");
            Assert.Equal("Short", pathDictionary[aSubdomainOptions.ServerCertificate]);
        }

        [Fact]
        public void ServerNameMatchingIsCaseInsensitive()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "Www.Example.Org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "Exact"
                        }
                    }
                },
                {
                    "*.Example.Org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "WildcardPrefix"
                        }
                    }
                }
            };

            var mockCertificateConfigLoader = new MockCertificateConfigLoader();
            var pathDictionary = mockCertificateConfigLoader.CertToPathDictionary;

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                mockCertificateConfigLoader,
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var wwwSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "wWw.eXample.oRg");
            Assert.Equal("Exact", pathDictionary[wwwSubdomainOptions.ServerCertificate]);

            var baSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "B.a.eXample.oRg");
            Assert.Equal("WildcardPrefix", pathDictionary[baSubdomainOptions.ServerCertificate]);

            var aSubdomainOptions = sniOptionsSelector.GetOptions(new MockConnectionContext(), "A.eXample.oRg");
            Assert.Equal("WildcardPrefix", pathDictionary[aSubdomainOptions.ServerCertificate]);
        }

        [Fact]
        public void GetOptionsThrowsAnAuthenticationExceptionIfThereIsNoMatchingSniSection()
        {
            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                new Dictionary<string, SniConfig>(),
                new MockCertificateConfigLoader(),
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var authExWithServerName = Assert.Throws<AuthenticationException>(() => sniOptionsSelector.GetOptions(new MockConnectionContext(), "example.org"));
            Assert.Equal(CoreStrings.FormatSniNotConfiguredForServerName("example.org", "TestEndpointName"), authExWithServerName.Message);

            var authExWithoutServerName = Assert.Throws<AuthenticationException>(() => sniOptionsSelector.GetOptions(new MockConnectionContext(), null));
            Assert.Equal(CoreStrings.FormatSniNotConfiguredToAllowNoServerName("TestEndpointName"), authExWithoutServerName.Message);
        }

        [Fact]
        public void WildcardOnlyMatchesNullServerNameDueToNoAlpn()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "*",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig
                        {
                            Path = "WildcardOnly"
                        }
                    }
                }
            };

            var mockCertificateConfigLoader = new MockCertificateConfigLoader();
            var pathDictionary = mockCertificateConfigLoader.CertToPathDictionary;

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                mockCertificateConfigLoader,
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), null);
            Assert.Equal("WildcardOnly", pathDictionary[options.ServerCertificate]);
        }

        [Fact]
        public void CachesSslServerAuthenticationOptions()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options1 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            var options2 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Same(options1, options2);
        }

        [Fact]
        public void ClonesSslServerAuthenticationOptionsIfAnOnAuthenticateCallbackIsDefined()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            SslServerAuthenticationOptions lastSeenSslOptions = null;

            var fallbackOptions = new HttpsConnectionAdapterOptions
            {
                OnAuthenticate = (context, sslOptions) =>
                {
                    lastSeenSslOptions = sslOptions;
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                fallbackOptions,
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options1 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Same(lastSeenSslOptions, options1);

            var options2 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Same(lastSeenSslOptions, options2);

            Assert.NotSame(options1, options2);
        }

        [Fact]
        public void ClonesSslServerAuthenticationOptionsIfTheFallbackServerCertificateSelectorIsUsed()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "selector.example.org",
                    new SniConfig()
                },
                {
                    "config.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var selectorCertificate = _x509Certificate2;

            var fallbackOptions = new HttpsConnectionAdapterOptions
            {
                ServerCertificate = new X509Certificate2(),
                ServerCertificateSelector = (context, serverName) => selectorCertificate
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                fallbackOptions,
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var selectorOptions1 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "selector.example.org");
            Assert.Same(selectorCertificate, selectorOptions1.ServerCertificate);

            var selectorOptions2 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "selector.example.org");
            Assert.Same(selectorCertificate, selectorOptions2.ServerCertificate);

            // The SslServerAuthenticationOptions were cloned because the cert came from the ServerCertificateSelector fallback.
            Assert.NotSame(selectorOptions1, selectorOptions2);

            var configOptions1 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "config.example.org");
            Assert.NotSame(selectorCertificate, configOptions1.ServerCertificate);

            var configOptions2 = sniOptionsSelector.GetOptions(new MockConnectionContext(), "config.example.org");
            Assert.NotSame(selectorCertificate, configOptions2.ServerCertificate);

            // The SslServerAuthenticationOptions don't need to be cloned if a static cert is defined in config for the given server name.
            Assert.Same(configOptions1, configOptions2);
        }

        [Fact]
        public void ConstructorThrowsInvalidOperationExceptionIfNoCertificateDefiniedInConfigOrFallback()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                { "www.example.org", new SniConfig() }
            };

            var ex = Assert.Throws<InvalidOperationException>(
                () => new SniOptionsSelector(
                    "TestEndpointName",
                    sniDictionary,
                    new MockCertificateConfigLoader(),
                    fallbackHttpsOptions: new HttpsConnectionAdapterOptions(),
                    fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                    logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>()));

            Assert.Equal(CoreStrings.NoCertSpecifiedNoDevelopmentCertificateFound, ex.Message);
        }

        [Fact]
        public void FallsBackToHttpsConnectionAdapterCertificate()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                { "www.example.org", new SniConfig() }
            };
            var fallbackOptions = new HttpsConnectionAdapterOptions
            {
                ServerCertificate = new X509Certificate2(TestResources.GetCertPath("aspnetdevcert.pfx"), "testPassword")
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                fallbackOptions,
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Same(fallbackOptions.ServerCertificate, options.ServerCertificate);
        }

        [Fact]
        public void FallsBackToHttpsConnectionAdapterServerCertificateSelectorOverServerCertificate()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                { "www.example.org", new SniConfig() }
            };

            var selectorCertificate = _x509Certificate2;

            var fallbackOptions = new HttpsConnectionAdapterOptions
            {
                ServerCertificate = new X509Certificate2(),
                ServerCertificateSelector = (context, serverName) => selectorCertificate
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                fallbackOptions,
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Same(selectorCertificate, options.ServerCertificate);
        }

        [Fact]
        public void PrefersHttpProtocolsDefinedInSniConfig()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Protocols = HttpProtocols.None,
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var mockConnectionContext = new MockConnectionContext();
            sniOptionsSelector.GetOptions(mockConnectionContext, "www.example.org");

            var httpProtocolsFeature = mockConnectionContext.Features.Get<HttpProtocolsFeature>();
            Assert.NotNull(httpProtocolsFeature);
            Assert.Equal(HttpProtocols.None, httpProtocolsFeature.HttpProtocols);
        }

        [Fact]
        public void ConfiguresAlpnBasedOnConfiguredHttpProtocols()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        // I'm not using Http1AndHttp2 or Http2 because I don't want to account for
                        // validation and normalization. Other tests cover that.
                        Protocols = HttpProtocols.Http1,
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.None,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            var alpnList = options.ApplicationProtocols;

            Assert.NotNull(alpnList);
            var protocol = Assert.Single(alpnList);
            Assert.Equal(SslApplicationProtocol.Http11, protocol);
        }

        [Fact]
        public void FallsBackToFallbackHttpProtocols()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions(),
                fallbackHttpProtocols: HttpProtocols.Http1,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var mockConnectionContext = new MockConnectionContext();
            sniOptionsSelector.GetOptions(mockConnectionContext, "www.example.org");

            var httpProtocolsFeature = mockConnectionContext.Features.Get<HttpProtocolsFeature>();
            Assert.NotNull(httpProtocolsFeature);
            Assert.Equal(HttpProtocols.Http1, httpProtocolsFeature.HttpProtocols);
        }

        [Fact]
        public void PrefersSslProtocolsDefinedInSniConfig()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls11,
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions
                {
                    SslProtocols = SslProtocols.Tls13
                },
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Equal(SslProtocols.Tls13 | SslProtocols.Tls11, options.EnabledSslProtocols);
        }

        [Fact]
        public void FallsBackToFallbackSslProtocols()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions
                {
                    SslProtocols = SslProtocols.Tls13
                },
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");
            Assert.Equal(SslProtocols.Tls13, options.EnabledSslProtocols);
        }


        [Fact]
        public void PrefersClientCertificateModeDefinedInSniConfig()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions
                {
                    ClientCertificateMode = ClientCertificateMode.AllowCertificate
                },
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");

            Assert.True(options.ClientCertificateRequired);

            Assert.NotNull(options.RemoteCertificateValidationCallback);
            // The RemoteCertificateValidationCallback should first check if the certificate is null and return false since it's required.
            Assert.False(options.RemoteCertificateValidationCallback(sender: null, certificate: null, chain: null, SslPolicyErrors.None));
        }

        [Fact]
        public void FallsBackToFallbackClientCertificateMode()
        {
            var sniDictionary = new Dictionary<string, SniConfig>
            {
                {
                    "www.example.org",
                    new SniConfig
                    {
                        Certificate = new CertificateConfig()
                    }
                }
            };

            var sniOptionsSelector = new SniOptionsSelector(
                "TestEndpointName",
                sniDictionary,
                new MockCertificateConfigLoader(),
                new HttpsConnectionAdapterOptions
                {
                    ClientCertificateMode = ClientCertificateMode.AllowCertificate
                },
                fallbackHttpProtocols: HttpProtocols.Http1AndHttp2,
                logger: Mock.Of<ILogger<HttpsConnectionMiddleware>>());

            var options = sniOptionsSelector.GetOptions(new MockConnectionContext(), "www.example.org");

            // Despite the confusing name, ClientCertificateRequired being true simply requests a certificate from the client, but doesn't require it.
            Assert.True(options.ClientCertificateRequired);

            Assert.NotNull(options.RemoteCertificateValidationCallback);
            // The RemoteCertificateValidationCallback should see we're in the AllowCertificate mode and return true.
            Assert.True(options.RemoteCertificateValidationCallback(sender: null, certificate: null, chain: null, SslPolicyErrors.None));
        }

        [Fact]
        public void CloneSslOptionsClonesAllProperties()
        {
            var propertyNames = typeof(SslServerAuthenticationOptions).GetProperties().Select(property => property.Name).ToList();

            CipherSuitesPolicy cipherSuitesPolicy = null;

            if (!OperatingSystem.IsWindows())
            {
                try
                {
                    // The CipherSuitesPolicy ctor throws a PlatformNotSupportedException on Windows.
                    cipherSuitesPolicy = new CipherSuitesPolicy(new[] { TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384 });
                }
                catch (PlatformNotSupportedException)
                {
                    // The CipherSuitesPolicy ctor throws a PlatformNotSupportedException on Ubuntu 16.04.
                    // I don't know exactly which other distros/versions throw PNEs, but it isn't super relevant to this test,
                    // so let's just swallow this exception.
                }
            }

            // Set options properties to non-default values to verify they're copied.
            var options = new SslServerAuthenticationOptions
            {
                // Defaults to true
                AllowRenegotiation = false,
                // Defaults to null
                ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http2 },
                // Defaults to X509RevocationMode.NoCheck
                CertificateRevocationCheckMode = X509RevocationMode.Offline,
                // Defaults to null
                CipherSuitesPolicy = cipherSuitesPolicy,
                // Defaults to false
                ClientCertificateRequired = true,
                // Defaults to SslProtocols.None
                EnabledSslProtocols = SslProtocols.Tls13 | SslProtocols.Tls11,
                // Defaults to EncryptionPolicy.RequireEncryption
                EncryptionPolicy = EncryptionPolicy.NoEncryption,
                // Defaults to null
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                // Defaults to null
                ServerCertificate = new X509Certificate2(),
                // Defaults to null
                ServerCertificateContext = SslStreamCertificateContext.Create(_x509Certificate2, additionalCertificates: null, offline: true),
                // Defaults to null
                ServerCertificateSelectionCallback = (sender, serverName) => null,
            };

            var clonedOptions = SniOptionsSelector.CloneSslOptions(options);

            Assert.NotSame(options, clonedOptions);

            Assert.Equal(options.AllowRenegotiation, clonedOptions.AllowRenegotiation);
            Assert.True(propertyNames.Remove(nameof(options.AllowRenegotiation)));

            // Ensure the List<SslApplicationProtocol> is also cloned since it could be modified by a user callback.
            Assert.NotSame(options.ApplicationProtocols, clonedOptions.ApplicationProtocols);
            Assert.Equal(Assert.Single(options.ApplicationProtocols), Assert.Single(clonedOptions.ApplicationProtocols));
            Assert.True(propertyNames.Remove(nameof(options.ApplicationProtocols)));

            Assert.Equal(options.CertificateRevocationCheckMode, clonedOptions.CertificateRevocationCheckMode);
            Assert.True(propertyNames.Remove(nameof(options.CertificateRevocationCheckMode)));

            Assert.Same(options.CipherSuitesPolicy, clonedOptions.CipherSuitesPolicy);
            Assert.True(propertyNames.Remove(nameof(options.CipherSuitesPolicy)));

            Assert.Equal(options.ClientCertificateRequired, clonedOptions.ClientCertificateRequired);
            Assert.True(propertyNames.Remove(nameof(options.ClientCertificateRequired)));

            Assert.Equal(options.EnabledSslProtocols, clonedOptions.EnabledSslProtocols);
            Assert.True(propertyNames.Remove(nameof(options.EnabledSslProtocols)));

            Assert.Equal(options.EncryptionPolicy, clonedOptions.EncryptionPolicy);
            Assert.True(propertyNames.Remove(nameof(options.EncryptionPolicy)));

            Assert.Same(options.RemoteCertificateValidationCallback, clonedOptions.RemoteCertificateValidationCallback);
            Assert.True(propertyNames.Remove(nameof(options.RemoteCertificateValidationCallback)));

            // Technically the ServerCertificate could be reset/reimported, but I'm hoping this is uncommon. Trying to clone the certificate and/or context seems risky.
            Assert.Same(options.ServerCertificate, clonedOptions.ServerCertificate);
            Assert.True(propertyNames.Remove(nameof(options.ServerCertificate)));

            Assert.Same(options.ServerCertificateContext, clonedOptions.ServerCertificateContext);
            Assert.True(propertyNames.Remove(nameof(options.ServerCertificateContext)));

            Assert.Same(options.ServerCertificateSelectionCallback, clonedOptions.ServerCertificateSelectionCallback);
            Assert.True(propertyNames.Remove(nameof(options.ServerCertificateSelectionCallback)));

            // Ensure we've checked every property. When new properties get added, we'll have to update this test along with the CloneSslOptions implementation.
            Assert.Empty(propertyNames);
        }

        private class MockCertificateConfigLoader : ICertificateConfigLoader
        {
            public Dictionary<object, string> CertToPathDictionary { get; } = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);

            public bool IsTestMock => true;

            public X509Certificate2 LoadCertificate(CertificateConfig certInfo, string endpointName)
            {
                if (certInfo is null)
                {
                    return null;
                }

                var cert = TestResources.GetTestCertificate();
                CertToPathDictionary.Add(cert, certInfo.Path);
                return cert;
            }
        }

        private class MockConnectionContext : ConnectionContext
        {
            public override IDuplexPipe Transport { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string ConnectionId { get; set; } = "MockConnectionId";
            public override IFeatureCollection Features { get; } = new FeatureCollection();
            public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
        }
    }
}
