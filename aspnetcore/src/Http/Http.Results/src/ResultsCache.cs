using Microsoft.AspNetCore.Http.HttpResults;

namespace Microsoft.AspNetCore.Http;

internal static partial class ResultsCache
{
    public static NotFound NotFound { get; } = new();
    public static UnauthorizedHttpResult Unauthorized { get; } = new();
    public static BadRequest BadRequest { get; } = new();
    public static Conflict Conflict { get; } = new();
    public static NoContent NoContent { get; } = new();
    public static Ok Ok { get; } = new();
    public static UnprocessableEntity UnprocessableEntity { get; } = new();
}
