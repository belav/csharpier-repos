using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ApplicationModelWebSite;

public class ActionDescriptionAttribute : Attribute, IActionModelConvention
{
    private readonly object _value;

    public ActionDescriptionAttribute(object value)
    {
        _value = value;
    }

    public void Apply(ActionModel model)
    {
        model.Properties["description"] = _value;
    }
}
