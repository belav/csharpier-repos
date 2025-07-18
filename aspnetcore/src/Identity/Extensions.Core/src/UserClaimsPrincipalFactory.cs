// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Shared;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides methods to create a claims principal for a given user.
/// </summary>
/// <typeparam name="TUser">The type used to represent a user.</typeparam>
public class UserClaimsPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser>
    where TUser : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserClaimsPrincipalFactory{TUser}"/> class.
    /// </summary>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/> to retrieve user information from.</param>
    /// <param name="optionsAccessor">The configured <see cref="IdentityOptions"/>.</param>
    public UserClaimsPrincipalFactory(
        UserManager<TUser> userManager,
        IOptions<IdentityOptions> optionsAccessor
    )
    {
        ArgumentNullThrowHelper.ThrowIfNull(userManager);
        if (optionsAccessor == null || optionsAccessor.Value == null)
        {
            ArgumentNullThrowHelper.ThrowIfNull(optionsAccessor);
            throw new ArgumentException(
                $"{nameof(optionsAccessor)} cannot wrap a null value.",
                nameof(optionsAccessor)
            );
        }
        UserManager = userManager;
        Options = optionsAccessor.Value;
    }

    /// <summary>
    /// Gets the <see cref="UserManager{TUser}"/> for this factory.
    /// </summary>
    /// <value>
    /// The current <see cref="UserManager{TUser}"/> for this factory instance.
    /// </value>
    public UserManager<TUser> UserManager { get; private set; }

    /// <summary>
    /// Gets the <see cref="IdentityOptions"/> for this factory.
    /// </summary>
    /// <value>
    /// The current <see cref="IdentityOptions"/> for this factory instance.
    /// </value>
    public IdentityOptions Options { get; private set; }

    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> from an user asynchronously.
    /// </summary>
    /// <param name="user">The user to create a <see cref="ClaimsPrincipal"/> from.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsPrincipal"/>.</returns>
    public virtual async Task<ClaimsPrincipal> CreateAsync(TUser user)
    {
        ArgumentNullThrowHelper.ThrowIfNull(user);
        var id = await GenerateClaimsAsync(user).ConfigureAwait(false);
        return new ClaimsPrincipal(id);
    }

    /// <summary>
    /// Generate the claims for a user.
    /// </summary>
    /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
    protected virtual async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
    {
        var userId = await UserManager.GetUserIdAsync(user).ConfigureAwait(false);
        var userName = await UserManager.GetUserNameAsync(user).ConfigureAwait(false);
        var id = new ClaimsIdentity(
            "Identity.Application", // REVIEW: Used to match Application scheme
            Options.ClaimsIdentity.UserNameClaimType,
            Options.ClaimsIdentity.RoleClaimType
        );
        id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
        id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userName!));
        if (UserManager.SupportsUserEmail)
        {
            var email = await UserManager.GetEmailAsync(user).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(email))
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.EmailClaimType, email));
            }
        }
        if (UserManager.SupportsUserSecurityStamp)
        {
            id.AddClaim(
                new Claim(
                    Options.ClaimsIdentity.SecurityStampClaimType,
                    await UserManager.GetSecurityStampAsync(user).ConfigureAwait(false)
                )
            );
        }
        if (UserManager.SupportsUserClaim)
        {
            id.AddClaims(await UserManager.GetClaimsAsync(user).ConfigureAwait(false));
        }
        return id;
    }
}

/// <summary>
/// Provides methods to create a claims principal for a given user.
/// </summary>
/// <typeparam name="TUser">The type used to represent a user.</typeparam>
/// <typeparam name="TRole">The type used to represent a role.</typeparam>
public class UserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser>
    where TUser : class
    where TRole : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserClaimsPrincipalFactory{TUser, TRole}"/> class.
    /// </summary>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/> to retrieve user information from.</param>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> to retrieve a user's roles from.</param>
    /// <param name="options">The configured <see cref="IdentityOptions"/>.</param>
    public UserClaimsPrincipalFactory(
        UserManager<TUser> userManager,
        RoleManager<TRole> roleManager,
        IOptions<IdentityOptions> options
    )
        : base(userManager, options)
    {
        ArgumentNullThrowHelper.ThrowIfNull(roleManager);
        RoleManager = roleManager;
    }

    /// <summary>
    /// Gets the <see cref="RoleManager{TRole}"/> for this factory.
    /// </summary>
    /// <value>
    /// The current <see cref="RoleManager{TRole}"/> for this factory instance.
    /// </value>
    public RoleManager<TRole> RoleManager { get; private set; }

    /// <summary>
    /// Generate the claims for a user.
    /// </summary>
    /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
    {
        var id = await base.GenerateClaimsAsync(user).ConfigureAwait(false);
        if (UserManager.SupportsUserRole)
        {
            var roles = await UserManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (var roleName in roles)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                if (RoleManager.SupportsRoleClaims)
                {
                    var role = await RoleManager.FindByNameAsync(roleName).ConfigureAwait(false);
                    if (role != null)
                    {
                        id.AddClaims(await RoleManager.GetClaimsAsync(role).ConfigureAwait(false));
                    }
                }
            }
        }
        return id;
    }
}
