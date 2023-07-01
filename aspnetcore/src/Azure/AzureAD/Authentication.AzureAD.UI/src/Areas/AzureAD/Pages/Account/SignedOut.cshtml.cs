using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Microsoft.AspNetCore.Authentication.AzureAD.UI.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code.This API may change or be removed in future releases
/// </summary>
[AllowAnonymous]
public class SignedOutModel : PageModel
{
    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code.This API may change or be removed in future releases
    /// </summary>
    public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            return LocalRedirect("~/");
        }

        return Page();
    }
}
