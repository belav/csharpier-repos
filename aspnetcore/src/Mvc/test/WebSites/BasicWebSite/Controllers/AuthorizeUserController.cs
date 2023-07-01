using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite;

[Authorize("Api")]
public class AuthorizeUserController : Controller
{
    [Authorize("Api-Manager")]
    public string ApiManagers()
    {
        return "Hello World!";
    }

    [Authorize(Roles = "Administrator")]
    public string AdminRole()
    {
        return "Hello World!";
    }

    [Authorize("Impossible")]
    public string Impossible()
    {
        throw new Exception("Shouldn't be invoked.");
    }
}
