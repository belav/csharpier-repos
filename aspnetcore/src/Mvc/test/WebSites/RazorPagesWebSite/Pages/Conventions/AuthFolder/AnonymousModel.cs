// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

[AllowAnonymous]
public class AnonymousModel : PageModel
{
    public void OnGet() { }
}
