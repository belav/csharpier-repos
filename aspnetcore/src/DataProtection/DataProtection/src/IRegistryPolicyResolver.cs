using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.DataProtection;

// Single implementation of this interface is conditionally added to DI on Windows
// We have to use interface because some DI implementations would try to activate class
// even if it was not registered causing problems crossplat
internal interface IRegistryPolicyResolver
{
    RegistryPolicy? ResolvePolicy();
}
