using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace RazorWebSite.Controllers;

public class AddTagHelperComponentController : Controller
{
    private readonly ITagHelperComponentManager _tagHelperComponentManager;

    public AddTagHelperComponentController(ITagHelperComponentManager tagHelperComponentManager)
    {
        _tagHelperComponentManager = tagHelperComponentManager;
    }

    public IActionResult AddComponent()
    {
        _tagHelperComponentManager.Components.Add(
            new TestBodyTagHelperComponent(0, "Processed TagHelperComponent added from controller.")
        );
        ViewData["TestData"] = "Value";
        return View("AddComponent");
    }
}
