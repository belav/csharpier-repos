// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication;

partial public class RemoteAuthenticatorViewCore<TAuthenticationState>
    where TAuthenticationState : RemoteAuthenticationState
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "Processing action {Action}.",
            EventName = nameof(ProcessingAuthenticatorAction)
        )]
        partial public static void ProcessingAuthenticatorAction(ILogger logger, string? action);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Login completed successfully.",
            EventName = nameof(LoginCompletedSuccessfully)
        )]
        partial public static void LoginCompletedSuccessfully(ILogger logger);

        [LoggerMessage(
            3,
            LogLevel.Debug,
            "Login requires redirect to the identity provider.",
            EventName = nameof(LoginRequiresRedirect)
        )]
        partial public static void LoginRequiresRedirect(ILogger logger);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            "Navigating to {Url}.",
            EventName = nameof(NavigatingToUrl)
        )]
        partial public static void NavigatingToUrl(
            ILogger logger,
            [StringSyntax(StringSyntaxAttribute.Uri)] string url
        );

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Raising LoginCompleted event.",
            EventName = nameof(InvokingLoginCompletedCallback)
        )]
        partial public static void InvokingLoginCompletedCallback(ILogger logger);

        [LoggerMessage(
            6,
            LogLevel.Debug,
            "Login operation failed with error '{ErrorMessage}'.",
            EventName = nameof(LoginFailed)
        )]
        partial public static void LoginFailed(ILogger logger, string errorMessage);

        [LoggerMessage(
            7,
            LogLevel.Debug,
            "Login callback failed with error '{ErrorMessage}'.",
            EventName = nameof(LoginCallbackFailed)
        )]
        partial public static void LoginCallbackFailed(ILogger logger, string errorMessage);

        [LoggerMessage(
            8,
            LogLevel.Debug,
            "Login redirect completed successfully.",
            EventName = nameof(LoginRedirectCompletedSuccessfully)
        )]
        partial public static void LoginRedirectCompletedSuccessfully(ILogger logger);

        [LoggerMessage(
            9,
            LogLevel.Debug,
            "The logout was not initiated from within the page.",
            EventName = nameof(LogoutOperationInitiatedExternally)
        )]
        partial public static void LogoutOperationInitiatedExternally(ILogger logger);

        [LoggerMessage(
            10,
            LogLevel.Debug,
            "Login completed successfully.",
            EventName = nameof(LoginCompletedSuccessfully)
        )]
        partial public static void LogoutCompletedSuccessfully(ILogger logger);

        [LoggerMessage(
            11,
            LogLevel.Debug,
            "Logout requires redirect to the identity provider.",
            EventName = nameof(LogoutRequiresRedirect)
        )]
        partial public static void LogoutRequiresRedirect(ILogger logger);

        [LoggerMessage(
            12,
            LogLevel.Debug,
            "Raising LogoutCompleted event.",
            EventName = nameof(InvokingLogoutCompletedCallback)
        )]
        partial public static void InvokingLogoutCompletedCallback(ILogger logger);

        [LoggerMessage(
            13,
            LogLevel.Debug,
            "Logout operation failed with error '{ErrorMessage}'.",
            EventName = nameof(LogoutFailed)
        )]
        partial public static void LogoutFailed(ILogger logger, string errorMessage);

        [LoggerMessage(
            14,
            LogLevel.Debug,
            "Logout callback failed with error '{ErrorMessage}'.",
            EventName = nameof(LogoutCallbackFailed)
        )]
        partial public static void LogoutCallbackFailed(ILogger logger, string errorMessage);

        [LoggerMessage(
            15,
            LogLevel.Debug,
            "Logout redirect completed successfully.",
            EventName = nameof(LogoutRedirectCompletedSuccessfully)
        )]
        partial public static void LogoutRedirectCompletedSuccessfully(ILogger logger);
    }
}
