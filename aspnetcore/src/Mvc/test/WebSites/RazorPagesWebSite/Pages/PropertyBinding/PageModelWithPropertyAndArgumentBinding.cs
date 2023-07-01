using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite;

public class PageModelWithPropertyAndArgumentBinding : PageModel
{
    [ModelBinder]
    public UserModel UserModel { get; set; }

    public int Id { get; set; }

    public void OnGet(int id)
    {
        Id = id;
    }

    public void OnPost(int id)
    {
        Id = id;
    }
}
