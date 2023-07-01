using System.Security.Principal;

namespace Microsoft.AspNetCore.Authentication.Negotiate;

// For testing
internal interface INegotiateState : IDisposable
{
    string? GetOutgoingBlob(string incomingBlob, out BlobErrorType status, out Exception? error);

    bool IsCompleted { get; }

    string Protocol { get; }

    IIdentity GetIdentity();
}

internal enum BlobErrorType
{
    None,
    CredentialError,
    ClientError,
    Other
}
