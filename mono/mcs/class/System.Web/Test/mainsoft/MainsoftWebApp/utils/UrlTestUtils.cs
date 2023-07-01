using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace GHTTests
{
    public class UrlTestUtils
    {
        public static string FixUrlForDirectoriesTest(string url)
        {
            if (url == null)
                return null;

            return url.Replace(HttpContext.Current.Request.ApplicationPath, "root");
        }
    }
}
