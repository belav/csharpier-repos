using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace Test_09.Tests
{
    public class PreStart
    {
        public static void FormsAuthenticationSetUp()
        {
            var nvc = new NameValueCollection();

            nvc.Add("loginUrl", "/myLogin.aspx");
            nvc.Add("defaultUrl", "/myDefault.aspx");
            FormsAuthentication.EnableFormsAuthentication(nvc);
        }
    }
}
