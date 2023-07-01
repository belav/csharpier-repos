using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ApplicationModelWebSite;

public class ControllerDescriptionAttribute : Attribute, IControllerModelConvention
{
    private readonly object _value;

    public ControllerDescriptionAttribute(object value)
    {
        _value = value;
    }

    public void Apply(ControllerModel model)
    {
        model.Properties["description"] = _value;
    }
}
