using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Microsoft.AspNetCore.Mvc;

public interface IViewComponentHelper
{
    Task<IHtmlContent> InvokeAsync(string name, object arguments);

    Task<IHtmlContent> InvokeAsync(Type componentType, object arguments);
}
