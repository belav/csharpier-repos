// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Shared;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a token provider that generates tokens from a user's security stamp and
/// sends them to the user via their phone number.
/// </summary>
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
public class PhoneNumberTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser>
    where TUser : class
{
    /// <summary>
    /// Returns a flag indicating whether the token provider can generate a token suitable for two-factor authentication token for
    /// the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
    /// <param name="user">The user a token could be generated for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the a flag indicating if a two
    /// factor token could be generated by this provider for the specified <paramref name="user"/>.
    /// The task will return true if a two-factor authentication token could be generated as the user has
    /// a telephone number, otherwise false.
    /// </returns>
    public override async Task<bool> CanGenerateTwoFactorTokenAsync(
        UserManager<TUser> manager,
        TUser user
    )
    {
        ArgumentNullThrowHelper.ThrowIfNull(manager);

        var phoneNumber = await manager.GetPhoneNumberAsync(user).ConfigureAwait(false);

        return !string.IsNullOrWhiteSpace(phoneNumber)
            && await manager.IsPhoneNumberConfirmedAsync(user).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a constant, provider and user unique modifier used for entropy in generated tokens from user information.
    /// </summary>
    /// <param name="purpose">The purpose the token will be generated for.</param>
    /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
    /// <param name="user">The user a token should be generated for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing a constant modifier for the specified
    /// <paramref name="user"/> and <paramref name="purpose"/>.
    /// </returns>
    public override async Task<string> GetUserModifierAsync(
        string purpose,
        UserManager<TUser> manager,
        TUser user
    )
    {
        ArgumentNullThrowHelper.ThrowIfNull(manager);

        var phoneNumber = await manager.GetPhoneNumberAsync(user).ConfigureAwait(false);

        return $"PhoneNumber:{purpose}:{phoneNumber}";
    }
}
