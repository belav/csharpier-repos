using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Razor.TagHelpers;

public abstract class TagHelper : ITagHelper
{
    public virtual int Order { get; } = 0;

    public virtual void Init(TagHelperContext context) { }

    public virtual void Process(TagHelperContext context, TagHelperOutput output) { }

    public virtual Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        Process(context, output);
        return Task.CompletedTask;
    }
}
