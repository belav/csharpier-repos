using System.Net.Http;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account.Manage;

public class ResetAuthenticator : DefaultUIPage
{
    private readonly IHtmlFormElement _resetAuthenticatorForm;
    private readonly IHtmlElement _resetAuthenticatorButton;

    public ResetAuthenticator(
        HttpClient client,
        IHtmlDocument resetAuthenticator,
        DefaultUIContext context
    )
        : base(client, resetAuthenticator, context)
    {
        Assert.True(Context.UserAuthenticated);
        _resetAuthenticatorForm = HtmlAssert.HasForm(
            "#reset-authenticator-form",
            resetAuthenticator
        );
        _resetAuthenticatorButton = HtmlAssert.HasElement(
            "#reset-authenticator-button",
            resetAuthenticator
        );
    }

    internal async Task<ResetAuthenticator> ResetAuthenticatorAsync()
    {
        await Client.SendAsync(_resetAuthenticatorForm, _resetAuthenticatorButton);
        return this;
    }
}
