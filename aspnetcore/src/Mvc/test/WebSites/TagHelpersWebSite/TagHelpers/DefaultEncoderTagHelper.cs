using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TagHelpersWebSite.TagHelpers;

[HtmlTargetElement("pre")]
[HtmlTargetElement("inner")]
[OutputElementHint("pre")]
public class DefaultEncoderTagHelper : TagHelper
{
    public override int Order => 2;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var defaultContent = await output.GetChildContentAsync();

        output.Content.SetHtmlContent("Default encoder: ").AppendHtml(defaultContent);
        output.TagName = "pre";
    }
}
