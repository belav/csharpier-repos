using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite;

public class DataTokensController : Controller
{
    public object Index()
    {
        return RouteData.DataTokens;
    }
}
