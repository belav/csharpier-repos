using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

namespace Microsoft.AspNetCore.Mvc.Filters;

internal sealed class PageSaveTempDataPropertyFilter : SaveTempDataPropertyFilterBase, IPageFilter
{
    public PageSaveTempDataPropertyFilter(ITempDataDictionaryFactory factory)
        : base(factory) { }

    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        Subject = context.HandlerInstance;
        var tempData = _tempDataFactory.GetTempData(context.HttpContext);

        SetPropertyValues(tempData);
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
}
