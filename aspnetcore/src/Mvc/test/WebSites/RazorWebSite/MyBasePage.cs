using Microsoft.AspNetCore.Mvc.Razor;

namespace RazorWebSite;

public abstract class MyBasePage<TModel> : RazorPage<TModel>
{
    public override void WriteLiteral(object value)
    {
        base.WriteLiteral("WriteLiteral says:");
        base.WriteLiteral(value);
    }

    public override void WriteLiteral(string value)
    {
        base.WriteLiteral("WriteLiteral says:");
        base.WriteLiteral(value);
    }

    public override void Write(object value)
    {
        base.WriteLiteral("Write says:");
        base.Write(value);
    }

    public override void Write(string value)
    {
        base.WriteLiteral("Write says:");
        base.Write(value);
    }
}
