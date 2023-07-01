using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite;

[BindProperties(SupportsGet = true)]
public class BindPropertiesSupportsGetController : Controller
{
    public string Name { get; set; }

    public IActionResult Action() => Content(Name);
}
