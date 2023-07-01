using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Rewrite.IISUrlRewrite;

internal enum RedirectType
{
    Permanent = StatusCodes.Status301MovedPermanently,
    Found = StatusCodes.Status302Found,
    SeeOther = StatusCodes.Status303SeeOther,
    Temporary = StatusCodes.Status307TemporaryRedirect
}
