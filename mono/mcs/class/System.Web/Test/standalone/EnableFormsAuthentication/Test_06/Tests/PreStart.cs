using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace Test_06.Tests
{
    public class PreStart
    {
        public static void FormsAuthenticationSetUp()
        {
            var nvc = new NameValueCollection();

            nvc.Add("LoginURL", "/myLogin.aspx");
            nvc.Add("DefaultURL", "/myDefault.aspx");

            FormsAuthentication.EnableFormsAuthentication(nvc);
        }
    }
}
