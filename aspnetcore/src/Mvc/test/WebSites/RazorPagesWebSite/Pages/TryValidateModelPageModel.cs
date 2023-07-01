using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages;

public class TryValidateModelPageModel : PageModel
{
    [ModelBinder]
    public UserModel UserModel { get; set; }

    public bool Validate { get; set; }

    public void OnPost(UserModel user)
    {
        Validate = TryValidateModel(user);
    }
}
