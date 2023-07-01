using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc.Routing;

/// <summary>
/// Internal interfaces that allows us to optimize the request execution path based on ActionDescriptor
/// </summary>
internal interface IRequestDelegateFactory
{
    RequestDelegate? CreateRequestDelegate(
        ActionDescriptor actionDescriptor,
        RouteValueDictionary? dataTokens
    );
}
