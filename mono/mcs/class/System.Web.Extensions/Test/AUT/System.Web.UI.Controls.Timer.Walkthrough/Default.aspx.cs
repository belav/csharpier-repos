using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Walkthrough
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label2.Text = "Page created at: " + DateTime.Now.ToLongTimeString();
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Label1.Text = "Panel refreshed at: " + DateTime.Now.ToLongTimeString();
        }
    }
}
