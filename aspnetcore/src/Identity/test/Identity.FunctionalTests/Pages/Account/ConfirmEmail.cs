using System.Net.Http;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account;

public class ConfirmEmail : DefaultUIPage
{
    public ConfirmEmail(HttpClient client, IHtmlDocument document, DefaultUIContext context)
        : base(client, document, context) { }

    public static async Task<ConfirmEmail> Create(
        IHtmlAnchorElement link,
        HttpClient client,
        DefaultUIContext context
    )
    {
        var response = await client.GetAsync(link.Href);
        var confirmEmail = await ResponseAssert.IsHtmlDocumentAsync(response);

        return new ConfirmEmail(client, confirmEmail, context);
    }
}
