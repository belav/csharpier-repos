// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.Components.Authorization;

/// <summary>
/// Displays differing content depending on the user's authorization status.
/// </summary>
public class AuthorizeView : AuthorizeViewCore
{
    private readonly IAuthorizeData[] selfAsAuthorizeData;

    /// <summary>
    /// Constructs an instance of <see cref="AuthorizeView"/>.
    /// </summary>
    public AuthorizeView()
    {
        selfAsAuthorizeData = new[] { new AuthorizeDataAdapter(this) };
    }

    /// <summary>
    /// The policy name that determines whether the content can be displayed.
    /// </summary>
    [Parameter]
    public string? Policy { get; set; }

    /// <summary>
    /// A comma delimited list of roles that are allowed to display the content.
    /// </summary>
    [Parameter]
    public string? Roles { get; set; }

    /// <summary>
    /// Gets the data used for authorization.
    /// </summary>
    protected override IAuthorizeData[] GetAuthorizeData() => selfAsAuthorizeData;
}
