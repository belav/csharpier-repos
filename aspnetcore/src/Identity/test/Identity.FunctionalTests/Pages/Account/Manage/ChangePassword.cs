using System.Net.Http;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account.Manage;

public class ChangePassword : DefaultUIPage
{
    private readonly IHtmlFormElement _changePasswordForm;

    public ChangePassword(HttpClient client, IHtmlDocument changePassword, DefaultUIContext context)
        : base(client, changePassword, context)
    {
        _changePasswordForm = HtmlAssert.HasForm("#change-password-form", changePassword);
    }

    public async Task<ChangePassword> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        await Client.SendAsync(
            _changePasswordForm,
            new Dictionary<string, string>
            {
                ["Input_OldPassword"] = oldPassword,
                ["Input_NewPassword"] = newPassword,
                ["Input_ConfirmPassword"] = newPassword
            }
        );

        return this;
    }
}
