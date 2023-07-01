using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RazorWebSite;

public class TestHeadTagHelperComponent : TagHelperComponent
{
    public override int Order => 1;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (
            string.Equals(context.TagName, "head", StringComparison.Ordinal)
            && output.Attributes.ContainsName("inject")
        )
        {
            output.PostContent.AppendHtml("<script>'This was injected!!'</script>");
        }

        return Task.FromResult(0);
    }
}
