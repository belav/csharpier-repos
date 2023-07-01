using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesWebSite.Components;

public class ComponentFromServicesViewComponent : ViewComponent
{
    private readonly ValueService _value;

    public ComponentFromServicesViewComponent(ValueService value)
    {
        _value = value;
    }

    public string Invoke()
    {
        return $"Value = {_value.Value}";
    }
}
