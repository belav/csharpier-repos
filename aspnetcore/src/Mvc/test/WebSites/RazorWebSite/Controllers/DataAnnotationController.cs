using Microsoft.AspNetCore.Mvc;
using RazorWebSite.Models;

namespace RazorWebSite.Controllers;

public class EnumController : Controller
{
    public IActionResult Enum()
    {
        return View(new EnumModel { Id = ModelEnum.FirstOption });
    }
}
