// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting
{
    internal static class HostingLoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception?> _startupAssemblyLoaded =
                LoggerMessage.Define<string>(LogLevel.Debug, LoggerEventIds.HostingStartupAssemblyLoaded, "Loaded hosting startup assembly {assemblyName}", skipEnabledCheck: true);

        private static readonly Action<ILogger, string, Exception?> _listeningOnAddress =
                LoggerMessage.Define<string>(LogLevel.Information, LoggerEventIds.ServerListeningOnAddresses, "Now listening on: {address}");

        public static IDisposable RequestScope(this ILogger logger, HttpContext httpContext)
        {
            return logger.BeginScope(new HostingLogScope(httpContext));
        }

        public static void ListeningOnAddress(this ILogger logger, string address)
        {
            _listeningOnAddress(logger, address, null);
        }

        public static void StartupAssemblyLoaded(this ILogger logger, string assemblyName)
        {
            _startupAssemblyLoaded(logger, assemblyName, null);
        }

        public static void ApplicationError(this ILogger logger, Exception exception)
        {
            logger.ApplicationError(
                eventId: LoggerEventIds.ApplicationStartupException,
                message: "Application startup exception",
                exception: exception);
        }

        public static void HostingStartupAssemblyError(this ILogger logger, Exception exception)
        {
            logger.ApplicationError(
                eventId: LoggerEventIds.HostingStartupAssemblyException,
                message: "Hosting startup assembly exception",
                exception: exception);
        }

        public static void ApplicationError(this ILogger logger, EventId eventId, string message, Exception exception)
        {
            var reflectionTypeLoadException = exception as ReflectionTypeLoadException;
            if (reflectionTypeLoadException != null)
            {
                foreach (var ex in reflectionTypeLoadException.LoaderExceptions)
                {
                    if (ex != null)
                    {
                        message = message + Environment.NewLine + ex.Message;
                    }
                }
            }

            logger.LogCritical(
                eventId: eventId,
                message: message,
                exception: exception);
        }

        public static void Starting(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                   eventId: LoggerEventIds.Starting,
                   message: "Hosting starting");
            }
        }

        public static void Started(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    eventId: LoggerEventIds.Started,
                    message: "Hosting started");
            }
        }

        public static void Shutdown(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    eventId: LoggerEventIds.Shutdown,
                    message: "Hosting shutdown");
            }
        }

        public static void ServerShutdownException(this ILogger logger, Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(
                    eventId: LoggerEventIds.ServerShutdownException,
                    exception: ex,
                    message: "Server shutdown exception");
            }
        }

        private class HostingLogScope : IReadOnlyList<KeyValuePair<string, object>>
        {
            private readonly string _path;
            private readonly string _traceIdentifier;

            private string? _cachedToString;

            public int Count
            {
                get
                {
                    return 2;
                }
            }

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        return new KeyValuePair<string, object>("RequestId", _traceIdentifier);
                    }
                    else if (index == 1)
                    {
                        return new KeyValuePair<string, object>("RequestPath", _path);
                    }

                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            public HostingLogScope(HttpContext httpContext)
            {
                _traceIdentifier = httpContext.TraceIdentifier;
                _path = (httpContext.Request.PathBase.HasValue
                         ? httpContext.Request.PathBase + httpContext.Request.Path
                         : httpContext.Request.Path).ToString();
            }

            public override string ToString()
            {
                if (_cachedToString == null)
                {
                    _cachedToString = string.Format(
                        CultureInfo.InvariantCulture,
                        "RequestPath:{0} RequestId:{1}",
                        _path,
                        _traceIdentifier);
                }

                return _cachedToString;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

