// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BasicWebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json.Serialization;

namespace BasicWebSite.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CSharp7View()
    {
        var people = new List<(string FirstName, string LastName, object FavoriteNumber)>()
        {
            ("John", "Doe", 6.022_140_857_747_474e23),
            ("John", "Smith", 100_000_000_000),
            (
                "Someone",
                "Nice",
                (decimal)1.618_033_988_749_894_848_204_586_834_365_638_117_720_309_179M
            ),
        };

        return View(people);
    }

    // Keep the return type as object to ensure that we don't
    // wrap IActionResult instances into ObjectResults.
    public object PlainView()
    {
        return View();
    }

    public IActionResult ActionLinkView()
    {
        // This view contains a link generated with Html.ActionLink
        // that provides a host with non unicode characters.
        return View();
    }

    public IActionResult RedirectToActionReturningTaskAction()
    {
        return RedirectToAction("ActionReturningTask");
    }

    public IActionResult RedirectToRouteActionAsMethodAction()
    {
        return RedirectToRoute(
            "ActionAsMethod",
            new { action = "ActionReturningTask", controller = "Home" }
        );
    }

    public IActionResult RedirectToRouteUsingRouteName()
    {
        return RedirectToRoute("OrdersApi", new { id = 10 });
    }

    public IActionResult NoContentResult()
    {
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [AcceptVerbs("GET", "POST")]
    [RequireHttps]
    public IActionResult HttpsOnlyAction()
    {
        return Ok();
    }

    public Task ActionReturningTask()
    {
        Response.Headers.Add("Message", new[] { "Hello, World!" });
        return Task.FromResult(true);
    }

    public IActionResult JsonHelperInView()
    {
        Person person = new Person { id = 9000, FullName = "John <b>Smith</b>" };

        return View(person);
    }

    public IActionResult JsonHelperWithSettingsInView(bool snakeCase)
    {
        var person = new Person { id = 9000, FullName = "John <b>Smith</b>" };
        ViewData["naming"] = snakeCase
            ? (NamingStrategy)new SnakeCaseNamingStrategy()
            : new DefaultNamingStrategy();

        return View(person);
    }

    public IActionResult ViewWithPrefixedAttributeValue()
    {
        return View();
    }

    public string GetApplicationDescription()
    {
        return ControllerContext.ActionDescriptor.Properties["description"].ToString();
    }

    [HttpGet]
    public IActionResult Product()
    {
        return Content("Get Product");
    }

    [HttpPost]
    public IActionResult Product(Product product)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        return RedirectToAction();
    }

    [HttpGet]
    public IActionResult GetAssemblyPartData(
        [FromServices] ApplicationPartManager applicationPartManager
    )
    {
        // Ensures that the entry assembly part is marked correctly.
        var assemblyPartMetadata = applicationPartManager
            .ApplicationParts.OfType<AssemblyPart>()
            .Select(part => part.Name)
            .ToArray();

        return Ok(assemblyPartMetadata);
    }
}
