using Xunit;

// Workaround for DataProtectionProviderTests.System_UsesProvidedDirectoryAndCertificate
// https://github.com/aspnet/DataProtection/issues/160
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
