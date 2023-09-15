using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace GHTTests.System_Web_dll.PageDirectories.Controls1
{
    public partial class UserControl2 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Controls.Add(LoadControl("../Controls1/UserControl1.ascx"));
        }
    }
}
