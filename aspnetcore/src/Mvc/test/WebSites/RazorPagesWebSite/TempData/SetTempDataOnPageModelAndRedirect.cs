using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

public class SetTempDataOnPageModelAndRedirect : PageModel
{
    public IActionResult OnGet(string message)
    {
        TempData["Message"] = message;
        return Redirect("~/TempData/ShowMessage");
    }
}
