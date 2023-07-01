using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.TempData;

public class TempDataPageModel : PageModel
{
    [TempData]
    public string Message { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        Message = "Secret post";
        return Page();
    }
}
