using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecurityWebSite.Controllers;

public class AuthorizedActionsController : ControllerBase
{
    [AllowAnonymous]
    public IActionResult ActionWithoutAllowAnonymous() => Ok();

    public IActionResult ActionWithoutAuthAttribute() => Ok();

    [Authorize("RequireClaimB")]
    public IActionResult ActionWithAuthAttribute() => Ok();
}
