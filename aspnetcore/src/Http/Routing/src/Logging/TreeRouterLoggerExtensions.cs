using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Routing.Logging
{
    internal static class TreeRouterLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception?> _requestMatchedRoute;

        static TreeRouterLoggerExtensions()
        {
            _requestMatchedRoute = LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(1, "RequestMatchedRoute"),
                "Request successfully matched the route with name '{RouteName}' and template '{RouteTemplate}'"
            );
        }

        public static void RequestMatchedRoute(
            this ILogger logger,
            string routeName,
            string routeTemplate
        )
        {
            _requestMatchedRoute(logger, routeName, routeTemplate, null);
        }
    }
}
