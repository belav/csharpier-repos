using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace Test_08.Tests
{
    public class PreStart
    {
        public static void FormsAuthenticationSetUp()
        {
            var nvc = new NameValueCollection();

            nvc.Add("loginUrl", "/myLogin.aspx");
            nvc.Add("defaultUrl", "/myDefault.aspx");
            nvc.Add("cookieDomain", "MyCookieDomain.com");
            FormsAuthentication.EnableFormsAuthentication(nvc);

            nvc["loginUrl"] = "/myOtherLogin.aspx";
            nvc["defaultUrl"] = "/myOtherDefault.aspx";
            FormsAuthentication.EnableFormsAuthentication(nvc);
        }
    }
}
