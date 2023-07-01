using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides an abstraction used for personal data encryption.
/// </summary>
public interface IPersonalDataProtector
{
    /// <summary>
    /// Protect the data.
    /// </summary>
    /// <param name="data">The data to protect.</param>
    /// <returns>The protected data.</returns>
    [return: NotNullIfNotNull("data")]
    string? Protect(string? data);

    /// <summary>
    /// Unprotect the data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns>The unprotected data.</returns>
    [return: NotNullIfNotNull("data")]
    string? Unprotect(string? data);
}
