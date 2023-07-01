using BasicWebSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Controllers.ContentNegotiation;

[Route("ProducesWithMediaTypeSuffixesController/[action]")]
public class ProducesWithMediaTypeSuffixesController : Controller
{
    [Produces(
        "application/vnd.example.contact+json; v=2",
        "application/vnd.example.contact+xml; v=2"
    )]
    public Contact ContactInfo()
    {
        return new Contact() { Name = "Jason Ecsemelle" };
    }
}
