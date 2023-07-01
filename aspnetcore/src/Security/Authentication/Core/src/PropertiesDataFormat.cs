using Microsoft.AspNetCore.DataProtection;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// A <see cref="SecureDataFormat{TData}"/> instance to secure
/// <see cref="AuthenticationProperties"/>.
/// </summary>
public class PropertiesDataFormat : SecureDataFormat<AuthenticationProperties>
{
    /// <summary>
    /// Initializes a new instance of <see cref="PropertiesDataFormat"/>.
    /// </summary>
    /// <param name="protector">The <see cref="IDataProtector"/>.</param>
    public PropertiesDataFormat(IDataProtector protector)
        : base(new PropertiesSerializer(), protector) { }
}
