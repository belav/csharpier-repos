// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Certificates.Generation;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Tools.Internal;

namespace Microsoft.AspNetCore.DeveloperCertificates.Tools
{
    internal class Program
    {
        private const int CriticalError = -1;
        private const int Success = 0;
        private const int ErrorCreatingTheCertificate = 1;
        private const int ErrorSavingTheCertificate = 2;
        private const int ErrorExportingTheCertificate = 3;
        private const int ErrorTrustingTheCertificate = 4;
        private const int ErrorUserCancelledTrustPrompt = 5;
        private const int ErrorNoValidCertificateFound = 6;
        private const int ErrorCertificateNotTrusted = 7;
        private const int ErrorCleaningUpCertificates = 8;
        private const int InvalidCertificateState = 9;
        private const int InvalidKeyExportFormat = 10;
        private const int ErrorImportingCertificate = 11;
        private const int MissingCertificateFile = 12;
        private const int FailedToLoadCertificate = 13;
        private const int NoDevelopmentHttpsCertificate = 14;
        private const int ExistingCertificatesPresent = 15;

        private const string InvalidUsageErrorMessage = @"Incompatible set of flags. Sample usages
'dotnet dev-certs https'
'dotnet dev-certs https --clean'
'dotnet dev-certs https --clean --import ./certificate.pfx -p password'
'dotnet dev-certs https --check --trust'
'dotnet dev-certs https -ep ./certificate.pfx -p password --trust'
'dotnet dev-certs https -ep ./certificate.crt --trust --key-format Pem'
'dotnet dev-certs https -ep ./certificate.crt -p password --trust --key-format Pem'";

        public static readonly TimeSpan HttpsCertificateValidity = TimeSpan.FromDays(365);

        public static int Main(string[] args)
        {
            if (args.Contains("--debug"))
            {
                Console.WriteLine("Press any key to continue...");
                _ = Console.ReadKey();
                var newArgs = new List<string>(args);
                newArgs.Remove("--debug");
                args = newArgs.ToArray();
            }

            try
            {
                var app = new CommandLineApplication
                {
                    Name = "dotnet dev-certs"
                };

                app.Command("https", c =>
                {
                    var exportPath = c.Option("-ep|--export-path",
                        "Full path to the exported certificate",
                        CommandOptionType.SingleValue);

                    var password = c.Option("-p|--password",
                        "Password to use when exporting the certificate with the private key into a pfx file or to encrypt the Pem exported key",
                        CommandOptionType.SingleValue);

                    // We want to force generating a key without a password to not be an accident.
                    var noPassword = c.Option("-np|--no-password",
                        "Explicitly request that you don't use a password for the key when exporting a certificate to a PEM format",
                        CommandOptionType.NoValue);

                    var check = c.Option(
                        "-c|--check",
                        "Check for the existence of the certificate but do not perform any action",
                        CommandOptionType.NoValue);

                    var clean = c.Option(
                        "--clean",
                        "Cleans all HTTPS development certificates from the machine.",
                        CommandOptionType.NoValue);

                    var import = c.Option(
                        "-i|--import",
                        "Imports the provided HTTPS development certificate into the machine. All other HTTPS developer certificates will be cleared out",
                        CommandOptionType.SingleValue);

                    var format = c.Option(
                        "--format",
                        "Export the certificate in the given format. Valid values are Pfx and Pem. Pfx is the default.",
                        CommandOptionType.SingleValue);

                    CommandOption trust = null;
                    trust = c.Option("-t|--trust",
                        "Trust the certificate on the current platform",
                        CommandOptionType.NoValue);

                    var verbose = c.Option("-v|--verbose",
                        "Display more debug information.",
                        CommandOptionType.NoValue);

                    var quiet = c.Option("-q|--quiet",
                        "Display warnings and errors only.",
                        CommandOptionType.NoValue);

                    c.HelpOption("-h|--help");

                    c.OnExecute(() =>
                    {
                        var reporter = new ConsoleReporter(PhysicalConsole.Singleton, verbose.HasValue(), quiet.HasValue());

                        if (clean.HasValue())
                        {
                            if (exportPath.HasValue() || trust?.HasValue() == true || format.HasValue() || noPassword.HasValue() || check.HasValue() ||
                               (!import.HasValue() && password.HasValue()) ||
                               (import.HasValue() && !password.HasValue()))
                            {
                                reporter.Error(InvalidUsageErrorMessage);
                                return CriticalError;
                            }
                        }

                        if (check.HasValue())
                        {
                            if (exportPath.HasValue() || password.HasValue() || noPassword.HasValue() || clean.HasValue() || format.HasValue() || import.HasValue())
                            {
                                reporter.Error(InvalidUsageErrorMessage);
                                return CriticalError;
                            }
                        }

                        if (!clean.HasValue() && !check.HasValue())
                        {
                            if (password.HasValue() && noPassword.HasValue())
                            {
                                reporter.Error(InvalidUsageErrorMessage);
                                return CriticalError;
                            }

                            if (noPassword.HasValue() && !(format.HasValue() && string.Equals(format.Value(), "PEM", StringComparison.OrdinalIgnoreCase)))
                            {
                                reporter.Error(InvalidUsageErrorMessage);
                                return CriticalError;
                            }

                            if (import.HasValue())
                            {
                                reporter.Error(InvalidUsageErrorMessage);
                                return CriticalError;
                            }
                        }

                        if (check.HasValue())
                        {
                            return CheckHttpsCertificate(trust, reporter);
                        }

                        if (clean.HasValue())
                        {
                            var clean = CleanHttpsCertificates(reporter);
                            if (clean != Success || !import.HasValue())
                            {
                                return clean;
                            }

                            return ImportCertificate(import, password, reporter);
                        }

                        return EnsureHttpsCertificate(exportPath, password, noPassword, trust, format, reporter);
                    });
                });

                app.HelpOption("-h|--help");

                app.OnExecute(() =>
                {
                    app.ShowHelp();
                    return Success;
                });

                return app.Execute(args);
            }
            catch
            {
                return CriticalError;
            }
        }

        private static int ImportCertificate(CommandOption import, CommandOption password, ConsoleReporter reporter)
        {
            var manager = CertificateManager.Instance;
            try
            {
                var result = manager.ImportCertificate(import.Value(), password.Value());
                switch (result)
                {
                    case ImportCertificateResult.Succeeded:
                        reporter.Output("The certificate was successfully imported.");
                        break;
                    case ImportCertificateResult.CertificateFileMissing:
                        reporter.Error($"The certificate file '{import.Value()}' does not exist.");
                        return MissingCertificateFile;
                    case ImportCertificateResult.InvalidCertificate:
                        reporter.Error($"The provided certificate file '{import.Value()}' is not a valid PFX file or the password is incorrect.");
                        return FailedToLoadCertificate;
                    case ImportCertificateResult.NoDevelopmentHttpsCertificate:
                        reporter.Error($"The certificate at '{import.Value()}' is not a valid ASP.NET Core HTTPS development certificate.");
                        return NoDevelopmentHttpsCertificate;
                    case ImportCertificateResult.ExistingCertificatesPresent:
                        reporter.Error($"There are one or more ASP.NET Core HTTPS development certificates present in the environment. Remove them before importing the given certificate.");
                        return ExistingCertificatesPresent;
                    case ImportCertificateResult.ErrorSavingTheCertificateIntoTheCurrentUserPersonalStore:
                        reporter.Error("There was an error saving the HTTPS developer certificate to the current user personal certificate store.");
                        return ErrorSavingTheCertificate;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                return ErrorImportingCertificate;
            }

            return Success;
        }

        private static int CleanHttpsCertificates(IReporter reporter)
        {
            var manager = CertificateManager.Instance;
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    reporter.Output("Cleaning HTTPS development certificates from the machine. A prompt might get " +
                        "displayed to confirm the removal of some of the certificates.");
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    reporter.Output("Cleaning HTTPS development certificates from the machine. This operation might " +
                        "require elevated privileges. If that is the case, a prompt for credentials will be displayed.");
                }

                manager.CleanupHttpsCertificates();
                reporter.Output("HTTPS development certificates successfully removed from the machine.");
                return Success;
            }
            catch (Exception e)
            {
                reporter.Error("There was an error trying to clean HTTPS development certificates on this machine.");
                reporter.Error(e.Message);

                return ErrorCleaningUpCertificates;
            }
        }

        private static int CheckHttpsCertificate(CommandOption trust, IReporter reporter)
        {
            var now = DateTimeOffset.Now;
            var certificateManager = CertificateManager.Instance;
            var certificates = certificateManager.ListCertificates(StoreName.My, StoreLocation.CurrentUser, isValid: true);
            if (certificates.Count == 0)
            {
                reporter.Output("No valid certificate found.");
                return ErrorNoValidCertificateFound;
            }
            else
            {
                foreach (var certificate in certificates)
                {
                    // We never want check to require interaction.
                    // When IDEs run dotnet dev-certs https after calling --check, we will try to access the key and
                    // that will trigger a prompt if necessary.
                    var status = certificateManager.CheckCertificateState(certificate, interactive: false);
                    if (!status.Success)
                    {
                        reporter.Warn(status.FailureMessage);
                        return InvalidCertificateState;
                    }
                }
                reporter.Verbose("A valid certificate was found.");
            }

            if (trust != null && trust.HasValue())
            {
                if(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    var store = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? StoreName.My : StoreName.Root;
                    var trustedCertificates = certificateManager.ListCertificates(store, StoreLocation.CurrentUser, isValid: true);
                    if (!certificates.Any(c => certificateManager.IsTrusted(c)))
                    {
                        reporter.Output($@"The following certificates were found, but none of them is trusted:
    {string.Join(Environment.NewLine, certificates.Select(c => $"{c.Subject} - {c.Thumbprint}"))}");
                        return ErrorCertificateNotTrusted;
                    }
                    else
                    {
                        reporter.Output("A trusted certificate was found.");
                    }
                }
                else
                {
                    reporter.Warn("Checking the HTTPS development certificate trust status was requested. Checking whether the certificate is trusted or not is not supported on Linux distributions." +
                        "For instructions on how to manually validate the certificate is trusted on your Linux distribution, go to https://aka.ms/dev-certs-trust");
                }
            }

            return Success;
        }

        private static int EnsureHttpsCertificate(CommandOption exportPath, CommandOption password, CommandOption noPassword, CommandOption trust, CommandOption exportFormat, IReporter reporter)
        {
            var now = DateTimeOffset.Now;
            var manager = CertificateManager.Instance;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var certificates = manager.ListCertificates(StoreName.My, StoreLocation.CurrentUser, isValid: true, exportPath.HasValue());
                foreach (var certificate in certificates)
                {
                    var status = manager.CheckCertificateState(certificate, interactive: true);
                    if (!status.Success)
                    {
                        reporter.Warn("One or more certificates might be in an invalid state. We will try to access the certificate key " +
                            "for each certificate and as a result you might be prompted one or more times to enter " +
                            "your password to access the user keychain. " +
                            "When that happens, select 'Always Allow' to grant 'dotnet' access to the certificate key in the future.");
                    }

                    break;
                }
            }

            if (trust?.HasValue() == true)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    reporter.Warn("Trusting the HTTPS development certificate was requested. If the certificate is not " +
                        "already trusted we will run the following command:" + Environment.NewLine +
                        "'sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain <<certificate>>'" +
                        Environment.NewLine + "This command might prompt you for your password to install the certificate " +
                        "on the system keychain.");
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    reporter.Warn("Trusting the HTTPS development certificate was requested. A confirmation prompt will be displayed " +
                        "if the certificate was not previously trusted. Click yes on the prompt to trust the certificate.");
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    reporter.Warn("Trusting the HTTPS development certificate was requested. Trusting the certificate on Linux distributions automatically is not supported. " +
                        "For instructions on how to manually trust the certificate on your Linux distribution, go to https://aka.ms/dev-certs-trust");
                }
            }

            var format = CertificateKeyExportFormat.Pfx;
            if (exportFormat.HasValue() && !Enum.TryParse(exportFormat.Value(), ignoreCase: true, out format))
            {
                reporter.Error($"Unknown key format '{exportFormat.Value()}'.");
                return InvalidKeyExportFormat;
            }

            var result = manager.EnsureAspNetCoreHttpsDevelopmentCertificate(
                now,
                now.Add(HttpsCertificateValidity),
                exportPath.Value(),
                trust == null ? false : trust.HasValue() && !RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                password.HasValue() || (noPassword.HasValue() && format == CertificateKeyExportFormat.Pem),
                password.Value(),
                exportFormat.HasValue() ? format : CertificateKeyExportFormat.Pfx);

            switch (result)
            {
                case EnsureCertificateResult.Succeeded:
                    reporter.Output("The HTTPS developer certificate was generated successfully.");
                    if (exportPath.Value() != null)
                    {
                        reporter.Verbose($"The certificate was exported to {Path.GetFullPath(exportPath.Value())}");
                    }
                    return Success;
                case EnsureCertificateResult.ValidCertificatePresent:
                    reporter.Output("A valid HTTPS certificate is already present.");
                    if (exportPath.Value() != null)
                    {
                        reporter.Verbose($"The certificate was exported to {Path.GetFullPath(exportPath.Value())}");
                    }
                    return Success;
                case EnsureCertificateResult.ErrorCreatingTheCertificate:
                    reporter.Error("There was an error creating the HTTPS developer certificate.");
                    return ErrorCreatingTheCertificate;
                case EnsureCertificateResult.ErrorSavingTheCertificateIntoTheCurrentUserPersonalStore:
                    reporter.Error("There was an error saving the HTTPS developer certificate to the current user personal certificate store.");
                    return ErrorSavingTheCertificate;
                case EnsureCertificateResult.ErrorExportingTheCertificate:
                    reporter.Warn("There was an error exporting HTTPS developer certificate to a file.");
                    return ErrorExportingTheCertificate;
                case EnsureCertificateResult.FailedToTrustTheCertificate:
                    reporter.Warn("There was an error trusting HTTPS developer certificate.");
                    return ErrorTrustingTheCertificate;
                case EnsureCertificateResult.UserCancelledTrustStep:
                    reporter.Warn("The user cancelled the trust step.");
                    return ErrorUserCancelledTrustPrompt;
                default:
                    reporter.Error("Something went wrong. The HTTPS developer certificate could not be created.");
                    return CriticalError;
            }
        }
    }
}
