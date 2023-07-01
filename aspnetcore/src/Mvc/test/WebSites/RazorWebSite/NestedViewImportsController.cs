using Microsoft.AspNetCore.Mvc;

namespace RazorWebSite.Controllers;

public class NestedViewImportsController : Controller
{
    public ViewResult Index()
    {
        var model = new Person { Name = "Controller-Person" };

        return View("~/Views/NestedViewImports/Nested/Index.cshtml", model);
    }
}
