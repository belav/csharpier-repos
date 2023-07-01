using Microsoft.AspNetCore.DataProtection;

namespace Microsoft.AspNetCore.Authentication;

/// <summary>
/// A <see cref="SecureDataFormat{TData}"/> instance to secure
/// <see cref="AuthenticationTicket"/>.
/// </summary>
public class TicketDataFormat : SecureDataFormat<AuthenticationTicket>
{
    /// <summary>
    /// Initializes a new instance of <see cref="TicketDataFormat"/>.
    /// </summary>
    /// <param name="protector">The <see cref="IDataProtector"/>.</param>
    public TicketDataFormat(IDataProtector protector)
        : base(TicketSerializer.Default, protector) { }
}
