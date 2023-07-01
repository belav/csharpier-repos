using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

internal sealed class ControllerSaveTempDataPropertyFilter
    : SaveTempDataPropertyFilterBase,
        IActionFilter
{
    public ControllerSaveTempDataPropertyFilter(ITempDataDictionaryFactory factory)
        : base(factory) { }

    public void OnActionExecuted(ActionExecutedContext context) { }

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Subject = context.Controller;
        var tempData = _tempDataFactory.GetTempData(context.HttpContext);

        SetPropertyValues(tempData);
    }
}
