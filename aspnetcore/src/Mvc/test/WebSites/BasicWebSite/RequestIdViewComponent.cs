using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite;

public class RequestIdViewComponent : ViewComponent
{
    public RequestIdViewComponent(RequestIdService requestIdService)
    {
        RequestIdService = requestIdService;
    }

    private RequestIdService RequestIdService { get; }

    public IViewComponentResult Invoke()
    {
        return Content(RequestIdService.RequestId);
    }
}
