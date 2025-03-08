using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyLoginPage : Page
{
    protected void loginControl_Authenticate(object sender, AuthenticateEventArgs e)
    {
        e.Authenticated = FormsAuthentication.Authenticate(
            loginControl.UserName,
            loginControl.Password
        );
    }
}
