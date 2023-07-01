using Microsoft.AspNetCore.Mvc;

namespace ApplicationModelWebSite.Controllers;

public class LicenseController : Controller
{
    [HttpGet("License/GetLicense")]
    public string GetLicense()
    {
        return ControllerContext.ActionDescriptor.Properties["license"].ToString();
    }
}
