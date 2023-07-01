using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MvcSandbox;

public class PagesHome : PageModel
{
    public string Name { get; private set; } = "World";

    public IActionResult OnPost(string name)
    {
        Name = name;
        return Page();
    }
}
