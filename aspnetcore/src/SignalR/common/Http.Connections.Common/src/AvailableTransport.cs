using System.Collections.Generic;

namespace Microsoft.AspNetCore.Http.Connections;

/// <summary>
/// Part of the <see cref="NegotiationResponse"/> that represents an individual transport and the trasfer formats the transport supports.
/// </summary>
public class AvailableTransport
{
    /// <summary>
    /// A transport available on the server.
    /// </summary>
    public string? Transport { get; set; }

    /// <summary>
    /// A list of formats supported by the transport. Examples include "Text" and "Binary".
    /// </summary>
    public IList<string>? TransferFormats { get; set; }
}
