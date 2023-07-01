using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[ApiExplorerSettings(GroupName = "SetOnController")]
[Route("ApiExplorerNameSetExplicitly")]
public class ApiExplorerNameSetExplicitlyController : Controller
{
    [HttpGet("SetOnController")]
    public void SetOnController() { }

    [ApiExplorerSettings(GroupName = "SetOnAction")]
    [HttpGet("SetOnAction")]
    public void SetOnAction() { }
}
