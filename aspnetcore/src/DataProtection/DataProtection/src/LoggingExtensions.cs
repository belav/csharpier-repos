// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Microsoft.Extensions.Logging;

partial
/// <summary>
/// Helpful extension methods on <see cref="ILogger"/>.
/// </summary>
internal static class LoggingExtensions
{
    /// <summary>
    /// Returns a value stating whether the 'debug' log level is enabled.
    /// Returns false if the logger instance is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDebugLevelEnabled([NotNullWhen(true)] this ILogger? logger)
    {
        return IsLogLevelEnabledCore(logger, LogLevel.Debug);
    }

    /// <summary>
    /// Returns a value stating whether the 'trace' log level is enabled.
    /// Returns false if the logger instance is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTraceLevelEnabled([NotNullWhen(true)] this ILogger? logger)
    {
        return IsLogLevelEnabledCore(logger, LogLevel.Trace);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLogLevelEnabledCore([NotNullWhen(true)] ILogger? logger, LogLevel level)
    {
        return (logger != null && logger.IsEnabled(level));
    }

    [LoggerMessage(
        1,
        LogLevel.Warning,
        "Policy resolution states that a new key should be added to the key ring, but automatic generation of keys is disabled. Using fallback key {KeyId:B} with expiration {ExpirationDate:u} as default key.",
        EventName = "UsingFallbackKeyWithExpirationAsDefaultKey"
    )]
    partial public static void UsingFallbackKeyWithExpirationAsDefaultKey(
        this ILogger logger,
        Guid keyId,
        DateTimeOffset expirationDate
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Using key {KeyId:B} as the default key.",
        EventName = "UsingKeyAsDefaultKey"
    )]
    partial public static void UsingKeyAsDefaultKey(this ILogger logger, Guid keyId);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Opening CNG algorithm '{HashAlgorithm}' from provider '{HashAlgorithmProvider}' with HMAC.",
        EventName = "OpeningCNGAlgorithmFromProviderWithHMAC"
    )]
    partial public static void OpeningCNGAlgorithmFromProviderWithHMAC(
        this ILogger logger,
        string hashAlgorithm,
        string? hashAlgorithmProvider
    );

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Opening CNG algorithm '{EncryptionAlgorithm}' from provider '{EncryptionAlgorithmProvider}' with chaining mode CBC.",
        EventName = "OpeningCNGAlgorithmFromProviderWithChainingModeCBC"
    )]
    partial public static void OpeningCNGAlgorithmFromProviderWithChainingModeCBC(
        this ILogger logger,
        string encryptionAlgorithm,
        string? encryptionAlgorithmProvider
    );

    [LoggerMessage(
        5,
        LogLevel.Trace,
        "Performing unprotect operation to key {KeyId:B} with purposes {Purposes}.",
        EventName = "PerformingUnprotectOperationToKeyWithPurposes"
    )]
    partial public static void PerformingUnprotectOperationToKeyWithPurposes(
        this ILogger logger,
        Guid keyId,
        string purposes
    );

    [LoggerMessage(
        6,
        LogLevel.Trace,
        "Key {KeyId:B} was not found in the key ring. Unprotect operation cannot proceed.",
        EventName = "KeyWasNotFoundInTheKeyRingUnprotectOperationCannotProceed"
    )]
    partial public static void KeyWasNotFoundInTheKeyRingUnprotectOperationCannotProceed(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        7,
        LogLevel.Debug,
        "Key {KeyId:B} was revoked. Caller requested unprotect operation proceed regardless.",
        EventName = "KeyWasRevokedCallerRequestedUnprotectOperationProceedRegardless"
    )]
    partial public static void KeyWasRevokedCallerRequestedUnprotectOperationProceedRegardless(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        8,
        LogLevel.Debug,
        "Key {KeyId:B} was revoked. Unprotect operation cannot proceed.",
        EventName = "KeyWasRevokedUnprotectOperationCannotProceed"
    )]
    partial public static void KeyWasRevokedUnprotectOperationCannotProceed(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        9,
        LogLevel.Debug,
        "Opening CNG algorithm '{EncryptionAlgorithm}' from provider '{EncryptionAlgorithmProvider}' with chaining mode GCM.",
        EventName = "OpeningCNGAlgorithmFromProviderWithChainingModeGCM"
    )]
    partial public static void OpeningCNGAlgorithmFromProviderWithChainingModeGCM(
        this ILogger logger,
        string encryptionAlgorithm,
        string? encryptionAlgorithmProvider
    );

    [LoggerMessage(
        10,
        LogLevel.Debug,
        "Using managed keyed hash algorithm '{FullName}'.",
        EventName = "UsingManagedKeyedHashAlgorithm"
    )]
    partial public static void UsingManagedKeyedHashAlgorithm(this ILogger logger, string fullName);

    [LoggerMessage(
        11,
        LogLevel.Debug,
        "Using managed symmetric algorithm '{FullName}'.",
        EventName = "UsingManagedSymmetricAlgorithm"
    )]
    partial public static void UsingManagedSymmetricAlgorithm(this ILogger logger, string fullName);

    [LoggerMessage(
        12,
        LogLevel.Warning,
        "Key {KeyId:B} is ineligible to be the default key because its {MethodName} method failed.",
        EventName = "KeyIsIneligibleToBeTheDefaultKeyBecauseItsMethodFailed"
    )]
    partial public static void KeyIsIneligibleToBeTheDefaultKeyBecauseItsMethodFailed(
        this ILogger logger,
        Guid keyId,
        string methodName,
        Exception exception
    );

    [LoggerMessage(
        13,
        LogLevel.Debug,
        "Considering key {KeyId:B} with expiration date {ExpirationDate:u} as default key.",
        EventName = "ConsideringKeyWithExpirationDateAsDefaultKey"
    )]
    partial public static void ConsideringKeyWithExpirationDateAsDefaultKey(
        this ILogger logger,
        Guid keyId,
        DateTimeOffset expirationDate
    );

    [LoggerMessage(
        14,
        LogLevel.Debug,
        "Key {KeyId:B} is no longer under consideration as default key because it is expired, revoked, or cannot be deciphered.",
        EventName = "KeyIsNoLongerUnderConsiderationAsDefault"
    )]
    partial public static void KeyIsNoLongerUnderConsiderationAsDefault(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        15,
        LogLevel.Warning,
        "Unknown element with name '{Name}' found in keyring, skipping.",
        EventName = "UnknownElementWithNameFoundInKeyringSkipping"
    )]
    partial public static void UnknownElementWithNameFoundInKeyringSkipping(
        this ILogger logger,
        XName name
    );

    [LoggerMessage(
        16,
        LogLevel.Debug,
        "Marked key {KeyId:B} as revoked in the keyring.",
        EventName = "MarkedKeyAsRevokedInTheKeyring"
    )]
    partial public static void MarkedKeyAsRevokedInTheKeyring(this ILogger logger, Guid keyId);

    [LoggerMessage(
        17,
        LogLevel.Warning,
        "Tried to process revocation of key {KeyId:B}, but no such key was found in keyring. Skipping.",
        EventName = "TriedToProcessRevocationOfKeyButNoSuchKeyWasFound"
    )]
    partial public static void TriedToProcessRevocationOfKeyButNoSuchKeyWasFound(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(18, LogLevel.Debug, "Found key {KeyId:B}.", EventName = "FoundKey")]
    partial public static void FoundKey(this ILogger logger, Guid keyId);

    [LoggerMessage(
        19,
        LogLevel.Debug,
        "Found revocation of all keys created prior to {RevocationDate:u}.",
        EventName = "FoundRevocationOfAllKeysCreatedPriorTo"
    )]
    partial public static void FoundRevocationOfAllKeysCreatedPriorTo(
        this ILogger logger,
        DateTimeOffset revocationDate
    );

    [LoggerMessage(
        20,
        LogLevel.Debug,
        "Found revocation of key {KeyId:B}.",
        EventName = "FoundRevocationOfKey"
    )]
    partial public static void FoundRevocationOfKey(this ILogger logger, Guid keyId);

    [LoggerMessage(
        21,
        LogLevel.Error,
        "An exception occurred while processing the revocation element '{RevocationElement}'. Cannot continue keyring processing.",
        EventName = "ExceptionWhileProcessingRevocationElement"
    )]
    partial public static void ExceptionWhileProcessingRevocationElement(
        this ILogger logger,
        XElement revocationElement,
        Exception exception
    );

    [LoggerMessage(
        22,
        LogLevel.Information,
        "Revoking all keys as of {RevocationDate:u} for reason '{Reason}'.",
        EventName = "RevokingAllKeysAsOfForReason"
    )]
    partial public static void RevokingAllKeysAsOfForReason(
        this ILogger logger,
        DateTimeOffset revocationDate,
        string? reason
    );

    [LoggerMessage(
        23,
        LogLevel.Debug,
        "Key cache expiration token triggered by '{OperationName}' operation.",
        EventName = "KeyCacheExpirationTokenTriggeredByOperation"
    )]
    partial public static void KeyCacheExpirationTokenTriggeredByOperation(
        this ILogger logger,
        string operationName
    );

    [LoggerMessage(
        24,
        LogLevel.Error,
        "An exception occurred while processing the key element '{Element}'.",
        EventName = "ExceptionOccurredWhileProcessingTheKeyElement"
    )]
    partial public static void ExceptionWhileProcessingKeyElement(
        this ILogger logger,
        XElement element,
        Exception exception
    );

    [LoggerMessage(
        25,
        LogLevel.Trace,
        "An exception occurred while processing the key element '{Element}'.",
        EventName = "ExceptionOccurredWhileProcessingTheKeyElementDebug"
    )]
    partial public static void AnExceptionOccurredWhileProcessingElementDebug(
        this ILogger logger,
        XElement element,
        Exception exception
    );

    [LoggerMessage(
        26,
        LogLevel.Debug,
        "Encrypting to Windows DPAPI for current user account ({Name}).",
        EventName = "EncryptingToWindowsDPAPIForCurrentUserAccount"
    )]
    partial public static void EncryptingToWindowsDPAPIForCurrentUserAccount(
        this ILogger logger,
        string name
    );

    [LoggerMessage(
        28,
        LogLevel.Error,
        "An error occurred while encrypting to X.509 certificate with thumbprint '{Thumbprint}'.",
        EventName = "ErrorOccurredWhileEncryptingToX509CertificateWithThumbprint"
    )]
    partial public static void AnErrorOccurredWhileEncryptingToX509CertificateWithThumbprint(
        this ILogger logger,
        string thumbprint,
        Exception exception
    );

    [LoggerMessage(
        29,
        LogLevel.Debug,
        "Encrypting to X.509 certificate with thumbprint '{Thumbprint}'.",
        EventName = "EncryptingToX509CertificateWithThumbprint"
    )]
    partial public static void EncryptingToX509CertificateWithThumbprint(
        this ILogger logger,
        string thumbprint
    );

    [LoggerMessage(
        30,
        LogLevel.Error,
        "An exception occurred while trying to resolve certificate with thumbprint '{Thumbprint}'.",
        EventName = "ExceptionOccurredWhileTryingToResolveCertificateWithThumbprint"
    )]
    partial public static void ExceptionWhileTryingToResolveCertificateWithThumbprint(
        this ILogger logger,
        string thumbprint,
        Exception exception
    );

    [LoggerMessage(
        31,
        LogLevel.Trace,
        "Performing protect operation to key {KeyId:B} with purposes {Purposes}.",
        EventName = "PerformingProtectOperationToKeyWithPurposes"
    )]
    partial public static void PerformingProtectOperationToKeyWithPurposes(
        this ILogger logger,
        Guid keyId,
        string purposes
    );

    [LoggerMessage(
        32,
        LogLevel.Debug,
        "Descriptor deserializer type for key {KeyId:B} is '{AssemblyQualifiedName}'.",
        EventName = "DescriptorDeserializerTypeForKeyIs"
    )]
    partial public static void DescriptorDeserializerTypeForKeyIs(
        this ILogger logger,
        Guid keyId,
        string assemblyQualifiedName
    );

    [LoggerMessage(
        33,
        LogLevel.Debug,
        "Key escrow sink found. Writing key {KeyId:B} to escrow.",
        EventName = "KeyEscrowSinkFoundWritingKeyToEscrow"
    )]
    partial public static void KeyEscrowSinkFoundWritingKeyToEscrow(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        34,
        LogLevel.Debug,
        "No key escrow sink found. Not writing key {KeyId:B} to escrow.",
        EventName = "NoKeyEscrowSinkFoundNotWritingKeyToEscrow"
    )]
    partial public static void NoKeyEscrowSinkFoundNotWritingKeyToEscrow(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        35,
        LogLevel.Warning,
        "No XML encryptor configured. Key {KeyId:B} may be persisted to storage in unencrypted form.",
        EventName = "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"
    )]
    partial public static void NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm(
        this ILogger logger,
        Guid keyId
    );

    [LoggerMessage(
        36,
        LogLevel.Information,
        "Revoking key {KeyId:B} at {RevocationDate:u} for reason '{Reason}'.",
        EventName = "RevokingKeyForReason"
    )]
    partial public static void RevokingKeyForReason(
        this ILogger logger,
        Guid keyId,
        DateTimeOffset revocationDate,
        string? reason
    );

    [LoggerMessage(
        37,
        LogLevel.Debug,
        "Reading data from file '{FullPath}'.",
        EventName = "ReadingDataFromFile"
    )]
    partial public static void ReadingDataFromFile(this ILogger logger, string fullPath);

    [LoggerMessage(
        38,
        LogLevel.Debug,
        "The name '{FriendlyName}' is not a safe file name, using '{NewFriendlyName}' instead.",
        EventName = "NameIsNotSafeFileName"
    )]
    partial public static void NameIsNotSafeFileName(
        this ILogger logger,
        string friendlyName,
        string newFriendlyName
    );

    [LoggerMessage(
        39,
        LogLevel.Information,
        "Writing data to file '{FileName}'.",
        EventName = "WritingDataToFile"
    )]
    partial public static void WritingDataToFile(this ILogger logger, string fileName);

    [LoggerMessage(
        40,
        LogLevel.Debug,
        "Reading data from registry key '{RegistryKeyName}', value '{Value}'.",
        EventName = "ReadingDataFromRegistryKeyValue"
    )]
    partial public static void ReadingDataFromRegistryKeyValue(
        this ILogger logger,
        RegistryKey registryKeyName,
        string value
    );

    [LoggerMessage(
        41,
        LogLevel.Debug,
        "The name '{FriendlyName}' is not a safe registry value name, using '{NewFriendlyName}' instead.",
        EventName = "NameIsNotSafeRegistryValueName"
    )]
    partial public static void NameIsNotSafeRegistryValueName(
        this ILogger logger,
        string friendlyName,
        string newFriendlyName
    );

    [LoggerMessage(
        42,
        LogLevel.Debug,
        "Decrypting secret element using Windows DPAPI-NG with protection descriptor rule '{DescriptorRule}'.",
        EventName = "DecryptingSecretElementUsingWindowsDPAPING"
    )]
    partial public static void DecryptingSecretElementUsingWindowsDPAPING(
        this ILogger logger,
        string? descriptorRule
    );

    [LoggerMessage(
        27,
        LogLevel.Debug,
        "Encrypting to Windows DPAPI-NG using protection descriptor rule '{DescriptorRule}'.",
        EventName = "EncryptingToWindowsDPAPINGUsingProtectionDescriptorRule"
    )]
    partial public static void EncryptingToWindowsDPAPINGUsingProtectionDescriptorRule(
        this ILogger logger,
        string descriptorRule
    );

    [LoggerMessage(
        43,
        LogLevel.Error,
        "An exception occurred while trying to decrypt the element.",
        EventName = "ExceptionOccurredTryingToDecryptElement"
    )]
    partial public static void ExceptionOccurredTryingToDecryptElement(
        this ILogger logger,
        Exception exception
    );

    [LoggerMessage(
        44,
        LogLevel.Warning,
        "Encrypting using a null encryptor; secret information isn't being protected.",
        EventName = "EncryptingUsingNullEncryptor"
    )]
    partial public static void EncryptingUsingNullEncryptor(this ILogger logger);

    [LoggerMessage(
        45,
        LogLevel.Information,
        "Using ephemeral data protection provider. Payloads will be undecipherable upon application shutdown.",
        EventName = "UsingEphemeralDataProtectionProvider"
    )]
    partial public static void UsingEphemeralDataProtectionProvider(this ILogger logger);

    [LoggerMessage(
        46,
        LogLevel.Debug,
        "Existing cached key ring is expired. Refreshing.",
        EventName = "ExistingCachedKeyRingIsExpiredRefreshing"
    )]
    partial public static void ExistingCachedKeyRingIsExpired(this ILogger logger);

    [LoggerMessage(
        47,
        LogLevel.Error,
        "An error occurred while refreshing the key ring. Will try again in 2 minutes.",
        EventName = "ErrorOccurredWhileRefreshingKeyRing"
    )]
    partial public static void ErrorOccurredWhileRefreshingKeyRing(
        this ILogger logger,
        Exception exception
    );

    [LoggerMessage(
        48,
        LogLevel.Error,
        "An error occurred while reading the key ring.",
        EventName = "ErrorOccurredWhileReadingKeyRing"
    )]
    partial public static void ErrorOccurredWhileReadingKeyRing(
        this ILogger logger,
        Exception exception
    );

    [LoggerMessage(
        49,
        LogLevel.Error,
        "The key ring does not contain a valid default key, and the key manager is configured with auto-generation of keys disabled.",
        EventName = "KeyRingDoesNotContainValidDefaultKey"
    )]
    partial public static void KeyRingDoesNotContainValidDefaultKey(this ILogger logger);

    [LoggerMessage(
        50,
        LogLevel.Warning,
        "Using an in-memory repository. Keys will not be persisted to storage.",
        EventName = "UsingInMemoryRepository"
    )]
    partial public static void UsingInmemoryRepository(this ILogger logger);

    [LoggerMessage(
        51,
        LogLevel.Debug,
        "Decrypting secret element using Windows DPAPI.",
        EventName = "DecryptingSecretElementUsingWindowsDPAPI"
    )]
    partial public static void DecryptingSecretElementUsingWindowsDPAPI(this ILogger logger);

    [LoggerMessage(
        52,
        LogLevel.Debug,
        "Default key expiration imminent and repository contains no viable successor. Caller should generate a successor.",
        EventName = "DefaultKeyExpirationImminentAndRepository"
    )]
    partial public static void DefaultKeyExpirationImminentAndRepository(this ILogger logger);

    [LoggerMessage(
        53,
        LogLevel.Debug,
        "Repository contains no viable default key. Caller should generate a key with immediate activation.",
        EventName = "RepositoryContainsNoViableDefaultKey"
    )]
    partial public static void RepositoryContainsNoViableDefaultKey(this ILogger logger);

    [LoggerMessage(
        54,
        LogLevel.Error,
        "An error occurred while encrypting to Windows DPAPI.",
        EventName = "ErrorOccurredWhileEncryptingToWindowsDPAPI"
    )]
    partial public static void ErrorOccurredWhileEncryptingToWindowsDPAPI(
        this ILogger logger,
        Exception exception
    );

    [LoggerMessage(
        55,
        LogLevel.Debug,
        "Encrypting to Windows DPAPI for local machine account.",
        EventName = "EncryptingToWindowsDPAPIForLocalMachineAccount"
    )]
    partial public static void EncryptingToWindowsDPAPIForLocalMachineAccount(this ILogger logger);

    [LoggerMessage(
        56,
        LogLevel.Error,
        "An error occurred while encrypting to Windows DPAPI-NG.",
        EventName = "ErrorOccurredWhileEncryptingToWindowsDPAPING"
    )]
    partial public static void ErrorOccurredWhileEncryptingToWindowsDPAPING(
        this ILogger logger,
        Exception exception
    );

    [LoggerMessage(
        57,
        LogLevel.Debug,
        "Policy resolution states that a new key should be added to the key ring.",
        EventName = "PolicyResolutionStatesThatANewKeyShouldBeAddedToTheKeyRing"
    )]
    partial public static void PolicyResolutionStatesThatANewKeyShouldBeAddedToTheKeyRing(
        this ILogger logger
    );

    [LoggerMessage(
        58,
        LogLevel.Information,
        "Creating key {KeyId:B} with creation date {CreationDate:u}, activation date {ActivationDate:u}, and expiration date {ExpirationDate:u}.",
        EventName = "CreatingKey"
    )]
    partial public static void CreatingKey(
        this ILogger logger,
        Guid keyId,
        DateTimeOffset creationDate,
        DateTimeOffset activationDate,
        DateTimeOffset expirationDate
    );

    [LoggerMessage(
        59,
        LogLevel.Warning,
        "Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits.",
        EventName = "UsingEphemeralKeyRepository"
    )]
    partial public static void UsingEphemeralKeyRepository(this ILogger logger);

    [LoggerMessage(
        61,
        LogLevel.Information,
        "User profile not available. Using '{Name}' as key repository and Windows DPAPI to encrypt keys at rest.",
        EventName = "UsingRegistryAsKeyRepositoryWithDPAPI"
    )]
    partial public static void UsingRegistryAsKeyRepositoryWithDPAPI(
        this ILogger logger,
        string name
    );

    [LoggerMessage(
        62,
        LogLevel.Information,
        "User profile is available. Using '{FullName}' as key repository; keys will not be encrypted at rest.",
        EventName = "UsingProfileAsKeyRepository"
    )]
    partial public static void UsingProfileAsKeyRepository(this ILogger logger, string fullName);

    [LoggerMessage(
        63,
        LogLevel.Information,
        "User profile is available. Using '{FullName}' as key repository and Windows DPAPI to encrypt keys at rest.",
        EventName = "UsingProfileAsKeyRepositoryWithDPAPI"
    )]
    partial public static void UsingProfileAsKeyRepositoryWithDPAPI(
        this ILogger logger,
        string fullName
    );

    [LoggerMessage(
        64,
        LogLevel.Information,
        "Azure Web Sites environment detected. Using '{FullName}' as key repository; keys will not be encrypted at rest.",
        EventName = "UsingAzureAsKeyRepository"
    )]
    partial public static void UsingAzureAsKeyRepository(this ILogger logger, string fullName);

    [LoggerMessage(
        65,
        LogLevel.Debug,
        "Key ring with default key {KeyId:B} was loaded during application startup.",
        EventName = "KeyRingWasLoadedOnStartup"
    )]
    partial public static void KeyRingWasLoadedOnStartup(this ILogger logger, Guid keyId);

    [LoggerMessage(
        66,
        LogLevel.Information,
        "Key ring failed to load during application startup.",
        EventName = "KeyRingFailedToLoadOnStartup"
    )]
    partial public static void KeyRingFailedToLoadOnStartup(
        this ILogger logger,
        Exception innerException
    );

    [LoggerMessage(
        60,
        LogLevel.Warning,
        "Storing keys in a directory '{path}' that may not be persisted outside of the container. Protected data will be unavailable when container is destroyed.",
        EventName = "UsingEphemeralFileSystemLocationInContainer"
    )]
    partial public static void UsingEphemeralFileSystemLocationInContainer(
        this ILogger logger,
        string path
    );
}
