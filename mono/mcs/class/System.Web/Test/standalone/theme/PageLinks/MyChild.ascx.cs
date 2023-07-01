using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

[Themeable(true)]
public partial class MyChild : System.Web.UI.UserControl
{
    protected override void OnInit(EventArgs e)
    {
        Debug.WriteLine("child.OnInit");
        base.OnInit(e);
    }

    [Themeable(true)]
    public string ImageUrl
    {
        get { return ((Image)Controls[0]).ImageUrl; }
        set
        {
            Debug.WriteLine("child.set_ImageUrl");
            ((Image)Controls[0]).ImageUrl = value;
        }
    }
}
