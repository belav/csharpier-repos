using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;

namespace Test_02.Tests
{
    public class PreStart
    {
        public static void FormsAuthenticationSetUp()
        {
            FormsAuthentication.EnableFormsAuthentication(new NameValueCollection());
        }
    }
}
