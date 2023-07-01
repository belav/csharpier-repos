using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BasicWebSite;

public class RequestScopedTagHelper : TagHelper
{
    public RequestScopedTagHelper(RequestIdService service)
    {
        RequestIdService = service;
    }

    public RequestIdService RequestIdService { get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Content.SetContent(RequestIdService.RequestId);
    }
}
