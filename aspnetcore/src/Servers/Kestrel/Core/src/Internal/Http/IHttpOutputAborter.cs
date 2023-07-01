using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

internal interface IHttpOutputAborter
{
    void Abort(ConnectionAbortedException abortReason);
    void OnInputOrOutputCompleted();
}
