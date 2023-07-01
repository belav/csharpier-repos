using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages;

public class TryUpdateModelPageModel : PageModel
{
    public UserModel UserModel { get; set; }

    public bool Updated { get; set; }

    public async Task OnPost()
    {
        var user = new UserModel();
        Updated = await TryUpdateModelAsync(user);
        UserModel = user;
    }
}
