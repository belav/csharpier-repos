using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

internal interface IAbsoluteUrlFactory
{
    string GetAbsoluteUrl(string path);
    string GetAbsoluteUrl(HttpContext context, string path);
}
