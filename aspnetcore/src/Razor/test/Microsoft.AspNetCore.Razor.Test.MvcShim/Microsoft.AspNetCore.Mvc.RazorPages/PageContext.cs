using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.AspNetCore.Mvc.RazorPages;

public class PageContext : ViewContext
{
    public Page Page { get; set; }
}
