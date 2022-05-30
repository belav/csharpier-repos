// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Provides the default validation of roles.
/// </summary>
/// <typeparam name="TRole">The type encapsulating a role.</typeparam>
public class RoleValidator<TRole> : IRoleValidator<TRole> where TRole : class
{
    /// <summary>
    /// Creates a new instance of <see cref="RoleValidator{TRole}"/>.
    /// </summary>
    /// <param name="errors">The <see cref="IdentityErrorDescriber"/> used to provider error messages.</param>
    public RoleValidator(IdentityErrorDescriber? errors = null)
    {
        Describer = errors ?? new IdentityErrorDescriber();
    }

    private IdentityErrorDescriber Describer { get; set; }

    /// <summary>
    /// Validates a role as an asynchronous operation.
    /// </summary>
    /// <param name="manager">The <see cref="RoleManager{TRole}"/> managing the role store.</param>
    /// <param name="role">The role to validate.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous validation.</returns>
    public virtual async Task<IdentityResult> ValidateAsync(RoleManager<TRole> manager, TRole role)
    {
        if (manager == null)
        {
            throw new ArgumentNullException(nameof(manager));
        }
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
        var errors = new List<IdentityError>();
        await ValidateRoleName(manager, role, errors).ConfigureAwait(false);
        if (errors.Count > 0)
        {
            return IdentityResult.Failed(errors.ToArray());
        }
        return IdentityResult.Success;
    }

    private async Task ValidateRoleName(RoleManager<TRole> manager, TRole role,
        ICollection<IdentityError> errors)
    {
        var roleName = await manager.GetRoleNameAsync(role).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(roleName))
        {
            errors.Add(Describer.InvalidRoleName(roleName));
        }
        else
        {
            var owner = await manager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (owner != null &&
                !string.Equals(await manager.GetRoleIdAsync(owner).ConfigureAwait(false), await manager.GetRoleIdAsync(role).ConfigureAwait(false)))
            {
                errors.Add(Describer.DuplicateRoleName(roleName));
            }
        }
    }
}
