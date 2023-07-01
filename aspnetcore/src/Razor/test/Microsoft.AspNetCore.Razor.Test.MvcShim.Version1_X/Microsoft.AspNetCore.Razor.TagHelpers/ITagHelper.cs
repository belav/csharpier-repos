using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Razor.TagHelpers;

public interface ITagHelper
{
    int Order { get; }

    void Init(TagHelperContext context);

    Task ProcessAsync(TagHelperContext context, TagHelperOutput output);
}
