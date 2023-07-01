using Microsoft.AspNetCore.Authentication;

namespace Identity.DefaultUI.WebSite;

public class ContosoAuthenticationOptions : AuthenticationSchemeOptions
{
    public ContosoAuthenticationOptions()
    {
        Events = new object();
    }

    public string SignInScheme { get; set; }
    public string ReturnUrlQueryParameter { get; set; } = "returnUrl";
    public string RemoteLoginPath { get; set; } = "/Contoso/Login";
}
