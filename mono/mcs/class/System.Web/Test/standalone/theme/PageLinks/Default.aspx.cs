using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class MyDefault : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected override void OnInit(EventArgs e)
    {
        Debug.WriteLine("page.OnInit");
        base.OnInit(e);
    }
}
