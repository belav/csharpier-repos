// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(
        13,
        LogLevel.Debug,
        "Updating configuration",
        EventName = "UpdatingConfiguration"
    )]
    partial public static void UpdatingConfiguration(this ILogger logger);

    [LoggerMessage(
        18,
        LogLevel.Debug,
        "Exception of type 'SecurityTokenSignatureKeyNotFoundException' thrown, Options.ConfigurationManager.RequestRefresh() called.",
        EventName = "ConfigurationManagerRequestRefreshCalled"
    )]
    partial public static void ConfigurationManagerRequestRefreshCalled(this ILogger logger);

    [LoggerMessage(
        27,
        LogLevel.Trace,
        "Authorization code received.",
        EventName = "AuthorizationCodeReceived"
    )]
    partial public static void AuthorizationCodeReceived(this ILogger logger);

    [LoggerMessage(
        30,
        LogLevel.Trace,
        "Token response received.",
        EventName = "TokenResponseReceived"
    )]
    partial public static void TokenResponseReceived(this ILogger logger);

    [LoggerMessage(21, LogLevel.Debug, "Received 'id_token'", EventName = "ReceivedIdToken")]
    partial public static void ReceivedIdToken(this ILogger logger);

    [LoggerMessage(
        19,
        LogLevel.Debug,
        "Redeeming code for tokens.",
        EventName = "RedeemingCodeForTokens"
    )]
    partial public static void RedeemingCodeForTokens(this ILogger logger);

    [LoggerMessage(
        15,
        LogLevel.Debug,
        "TokenValidated.HandledResponse",
        EventName = "TokenValidatedHandledResponse"
    )]
    partial public static void TokenValidatedHandledResponse(this ILogger logger);

    [LoggerMessage(
        16,
        LogLevel.Debug,
        "TokenValidated.Skipped",
        EventName = "TokenValidatedSkipped"
    )]
    partial public static void TokenValidatedSkipped(this ILogger logger);

    [LoggerMessage(
        28,
        LogLevel.Debug,
        "AuthorizationCodeReceivedContext.HandledResponse",
        EventName = "AuthorizationCodeReceivedContextHandledResponse"
    )]
    partial public static void AuthorizationCodeReceivedContextHandledResponse(this ILogger logger);

    [LoggerMessage(
        29,
        LogLevel.Debug,
        "AuthorizationCodeReceivedContext.Skipped",
        EventName = "AuthorizationCodeReceivedContextSkipped"
    )]
    partial public static void AuthorizationCodeReceivedContextSkipped(this ILogger logger);

    [LoggerMessage(
        31,
        LogLevel.Debug,
        "TokenResponseReceived.HandledResponse",
        EventName = "TokenResponseReceivedHandledResponse"
    )]
    partial public static void TokenResponseReceivedHandledResponse(this ILogger logger);

    [LoggerMessage(
        32,
        LogLevel.Debug,
        "TokenResponseReceived.Skipped",
        EventName = "TokenResponseReceivedSkipped"
    )]
    partial public static void TokenResponseReceivedSkipped(this ILogger logger);

    [LoggerMessage(
        38,
        LogLevel.Debug,
        "AuthenticationFailedContext.HandledResponse",
        EventName = "AuthenticationFailedContextHandledResponse"
    )]
    partial public static void AuthenticationFailedContextHandledResponse(this ILogger logger);

    [LoggerMessage(
        39,
        LogLevel.Debug,
        "AuthenticationFailedContext.Skipped",
        EventName = "AuthenticationFailedContextSkipped"
    )]
    partial public static void AuthenticationFailedContextSkipped(this ILogger logger);

    [LoggerMessage(
        24,
        LogLevel.Trace,
        "MessageReceived: '{RedirectUrl}'.",
        EventName = "MessageReceived"
    )]
    partial public static void MessageReceived(this ILogger logger, string redirectUrl);

    [LoggerMessage(
        25,
        LogLevel.Debug,
        "MessageReceivedContext.HandledResponse",
        EventName = "MessageReceivedContextHandledResponse"
    )]
    partial public static void MessageReceivedContextHandledResponse(this ILogger logger);

    [LoggerMessage(
        26,
        LogLevel.Debug,
        "MessageReceivedContext.Skipped",
        EventName = "MessageReceivedContextSkipped"
    )]
    partial public static void MessageReceivedContextSkipped(this ILogger logger);

    [LoggerMessage(
        1,
        LogLevel.Debug,
        "RedirectToIdentityProviderForSignOut.HandledResponse",
        EventName = "RedirectToIdentityProviderForSignOutHandledResponse"
    )]
    partial public static void RedirectToIdentityProviderForSignOutHandledResponse(
        this ILogger logger
    );

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "RedirectToIdentityProvider.HandledResponse",
        EventName = "RedirectToIdentityProviderHandledResponse"
    )]
    partial public static void RedirectToIdentityProviderHandledResponse(this ILogger logger);

    [LoggerMessage(
        50,
        LogLevel.Debug,
        "RedirectToSignedOutRedirectUri.HandledResponse",
        EventName = "SignOutCallbackRedirectHandledResponse"
    )]
    partial public static void SignOutCallbackRedirectHandledResponse(this ILogger logger);

    [LoggerMessage(
        51,
        LogLevel.Debug,
        "RedirectToSignedOutRedirectUri.Skipped",
        EventName = "SignOutCallbackRedirectSkipped"
    )]
    partial public static void SignOutCallbackRedirectSkipped(this ILogger logger);

    [LoggerMessage(
        36,
        LogLevel.Debug,
        "The UserInformationReceived event returned Handled.",
        EventName = "UserInformationReceivedHandledResponse"
    )]
    partial public static void UserInformationReceivedHandledResponse(this ILogger logger);

    [LoggerMessage(
        37,
        LogLevel.Debug,
        "The UserInformationReceived event returned Skipped.",
        EventName = "UserInformationReceivedSkipped"
    )]
    partial public static void UserInformationReceivedSkipped(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Warning,
        "The query string for Logout is not a well-formed URI. Redirect URI: '{RedirectUrl}'.",
        EventName = "InvalidLogoutQueryStringRedirectUrl"
    )]
    partial public static void InvalidLogoutQueryStringRedirectUrl(
        this ILogger logger,
        string redirectUrl
    );

    [LoggerMessage(
        10,
        LogLevel.Debug,
        "message.State is null or empty.",
        EventName = "NullOrEmptyAuthorizationResponseState"
    )]
    partial public static void NullOrEmptyAuthorizationResponseState(this ILogger logger);

    [LoggerMessage(
        11,
        LogLevel.Debug,
        "Unable to read the message.State.",
        EventName = "UnableToReadAuthorizationResponseState"
    )]
    partial public static void UnableToReadAuthorizationResponseState(this ILogger logger);

    [LoggerMessage(
        12,
        LogLevel.Error,
        "Message contains error: '{Error}', error_description: '{ErrorDescription}', error_uri: '{ErrorUri}'.",
        EventName = "ResponseError"
    )]
    partial public static void ResponseError(
        this ILogger logger,
        string error,
        string errorDescription,
        string errorUri
    );

    [LoggerMessage(
        52,
        LogLevel.Error,
        "Message contains error: '{Error}', error_description: '{ErrorDescription}', error_uri: '{ErrorUri}', status code '{StatusCode}'.",
        EventName = "ResponseErrorWithStatusCode"
    )]
    partial public static void ResponseErrorWithStatusCode(
        this ILogger logger,
        string error,
        string errorDescription,
        string errorUri,
        int statusCode
    );

    [LoggerMessage(
        17,
        LogLevel.Error,
        "Exception occurred while processing message.",
        EventName = "ExceptionProcessingMessage"
    )]
    partial public static void ExceptionProcessingMessage(this ILogger logger, Exception ex);

    [LoggerMessage(
        42,
        LogLevel.Debug,
        "The access_token is not available. Claims cannot be retrieved.",
        EventName = "AccessTokenNotAvailable"
    )]
    partial public static void AccessTokenNotAvailable(this ILogger logger);

    [LoggerMessage(
        20,
        LogLevel.Trace,
        "Retrieving claims from the user info endpoint.",
        EventName = "RetrievingClaims"
    )]
    partial public static void RetrievingClaims(this ILogger logger);

    [LoggerMessage(
        22,
        LogLevel.Debug,
        "UserInfoEndpoint is not set. Claims cannot be retrieved.",
        EventName = "UserInfoEndpointNotSet"
    )]
    partial public static void UserInfoEndpointNotSet(this ILogger logger);

    [LoggerMessage(
        23,
        LogLevel.Warning,
        "Failed to un-protect the nonce cookie.",
        EventName = "UnableToProtectNonceCookie"
    )]
    partial public static void UnableToProtectNonceCookie(this ILogger logger, Exception ex);

    [LoggerMessage(
        8,
        LogLevel.Warning,
        "The redirect URI is not well-formed. The URI is: '{AuthenticationRequestUrl}'.",
        EventName = "InvalidAuthenticationRequestUrl"
    )]
    partial public static void InvalidAuthenticationRequestUrl(
        this ILogger logger,
        string authenticationRequestUrl
    );

    [LoggerMessage(
        43,
        LogLevel.Error,
        "Unable to read the 'id_token', no suitable ISecurityTokenValidator was found for: '{IdToken}'.",
        EventName = "UnableToReadIdToken"
    )]
    partial public static void UnableToReadIdToken(this ILogger logger, string idToken);

    [LoggerMessage(
        40,
        LogLevel.Error,
        "The Validated Security Token must be of type JwtSecurityToken, but instead its type is: '{SecurityTokenType}'",
        EventName = "InvalidSecurityTokenType"
    )]
    partial public static void InvalidSecurityTokenType(
        this ILogger logger,
        string? securityTokenType
    );

    [LoggerMessage(
        41,
        LogLevel.Error,
        "Unable to validate the 'id_token', no suitable ISecurityTokenValidator was found for: '{IdToken}'.",
        EventName = "UnableToValidateIdToken"
    )]
    partial public static void UnableToValidateIdToken(this ILogger logger, string idToken);

    [LoggerMessage(
        9,
        LogLevel.Trace,
        "Entering {OpenIdConnectHandlerType}'s HandleRemoteAuthenticateAsync.",
        EventName = "EnteringOpenIdAuthenticationHandlerHandleRemoteAuthenticateAsync"
    )]
    partial public static void EnteringOpenIdAuthenticationHandlerHandleRemoteAuthenticateAsync(
        this ILogger logger,
        string openIdConnectHandlerType
    );

    [LoggerMessage(
        4,
        LogLevel.Trace,
        "Entering {OpenIdConnectHandlerType}'s HandleUnauthorizedAsync.",
        EventName = "EnteringOpenIdAuthenticationHandlerHandleUnauthorizedAsync"
    )]
    partial public static void EnteringOpenIdAuthenticationHandlerHandleUnauthorizedAsync(
        this ILogger logger,
        string openIdConnectHandlerType
    );

    [LoggerMessage(
        14,
        LogLevel.Trace,
        "Entering {OpenIdConnectHandlerType}'s HandleSignOutAsync.",
        EventName = "EnteringOpenIdAuthenticationHandlerHandleSignOutAsync"
    )]
    partial public static void EnteringOpenIdAuthenticationHandlerHandleSignOutAsync(
        this ILogger logger,
        string openIdConnectHandlerType
    );

    [LoggerMessage(
        35,
        LogLevel.Trace,
        "User information received: {User}",
        EventName = "UserInformationReceived"
    )]
    partial public static void UserInformationReceived(this ILogger logger, string user);

    [LoggerMessage(
        5,
        LogLevel.Trace,
        "Using properties.RedirectUri for 'local redirect' post authentication: '{RedirectUri}'.",
        EventName = "PostAuthenticationLocalRedirect"
    )]
    partial public static void PostAuthenticationLocalRedirect(
        this ILogger logger,
        string redirectUri
    );

    [LoggerMessage(
        33,
        LogLevel.Trace,
        "Using properties.RedirectUri for redirect post authentication: '{RedirectUri}'.",
        EventName = "PostSignOutRedirect"
    )]
    partial public static void PostSignOutRedirect(this ILogger logger, string redirectUri);

    [LoggerMessage(
        44,
        LogLevel.Debug,
        "RemoteSignOutContext.HandledResponse",
        EventName = "RemoteSignOutHandledResponse"
    )]
    partial public static void RemoteSignOutHandledResponse(this ILogger logger);

    [LoggerMessage(
        45,
        LogLevel.Debug,
        "RemoteSignOutContext.Skipped",
        EventName = "RemoteSignOutSkipped"
    )]
    partial public static void RemoteSignOutSkipped(this ILogger logger);

    [LoggerMessage(
        46,
        LogLevel.Information,
        "Remote signout request processed.",
        EventName = "RemoteSignOut"
    )]
    partial public static void RemoteSignOut(this ILogger logger);

    [LoggerMessage(
        47,
        LogLevel.Error,
        "The remote signout request was ignored because the 'sid' parameter "
            + "was missing, which may indicate an unsolicited logout.",
        EventName = "RemoteSignOutSessionIdMissing"
    )]
    partial public static void RemoteSignOutSessionIdMissing(this ILogger logger);

    [LoggerMessage(
        48,
        LogLevel.Error,
        "The remote signout request was ignored because the 'sid' parameter didn't match "
            + "the expected value, which may indicate an unsolicited logout.",
        EventName = "RemoteSignOutSessionIdInvalid"
    )]
    partial public static void RemoteSignOutSessionIdInvalid(this ILogger logger);

    [LoggerMessage(
        49,
        LogLevel.Information,
        "AuthenticationScheme: {AuthenticationScheme} signed out.",
        EventName = "AuthenticationSchemeSignedOut"
    )]
    partial public static void AuthenticationSchemeSignedOut(
        this ILogger logger,
        string authenticationScheme
    );

    [LoggerMessage(
        53,
        LogLevel.Debug,
        "HandleChallenge with Location: {Location}; and Set-Cookie: {Cookie}.",
        EventName = "HandleChallenge"
    )]
    partial public static void HandleChallenge(this ILogger logger, string location, string cookie);

    [LoggerMessage(
        54,
        LogLevel.Error,
        "The remote signout request was ignored because the 'iss' parameter "
            + "was missing, which may indicate an unsolicited logout.",
        EventName = "RemoteSignOutIssuerMissing"
    )]
    partial public static void RemoteSignOutIssuerMissing(this ILogger logger);

    [LoggerMessage(
        55,
        LogLevel.Error,
        "The remote signout request was ignored because the 'iss' parameter didn't match "
            + "the expected value, which may indicate an unsolicited logout.",
        EventName = "RemoteSignOutIssuerInvalid"
    )]
    partial public static void RemoteSignOutIssuerInvalid(this ILogger logger);
}
