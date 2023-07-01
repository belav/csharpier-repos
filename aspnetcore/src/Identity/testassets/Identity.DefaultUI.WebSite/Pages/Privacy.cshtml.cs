using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.DefaultUI.WebSite.Pages;

public class PrivacyModel : PageModel
{
    public void OnGet()
    {
        HttpContext.Features.Get<ITrackingConsentFeature>().GrantConsent();
    }
}
