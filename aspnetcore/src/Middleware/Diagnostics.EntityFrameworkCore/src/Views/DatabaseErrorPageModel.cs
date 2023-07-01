using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Views;

internal sealed class DatabaseErrorPageModel
{
    public DatabaseErrorPageModel(
        Exception exception,
        IEnumerable<DatabaseContextDetails> contextDetails,
        DatabaseErrorPageOptions options,
        PathString pathBase
    )
    {
        Exception = exception;
        ContextDetails = contextDetails;
        Options = options;
        PathBase = pathBase;
    }

    public Exception Exception { get; }
    public IEnumerable<DatabaseContextDetails> ContextDetails { get; }
    public DatabaseErrorPageOptions Options { get; }
    public PathString PathBase { get; }
}
