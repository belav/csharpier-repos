using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite.Controllers;

public class ApiExplorerInboundOutBoundController : Controller
{
    [HttpGet("ApiExplorerInboundOutbound/SuppressedForLinkGeneration")]
    public void SuppressedForLinkGeneration() { }

    [HttpGet("ApiExplorerInboundOutbound/SuppressedForPathMatching")]
    public void SuppressedForPathMatching() { }
}
