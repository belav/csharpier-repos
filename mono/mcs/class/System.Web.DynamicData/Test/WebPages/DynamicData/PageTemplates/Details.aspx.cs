using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Details : System.Web.UI.Page
{
    protected MetaTable table;

    protected void Page_Init(object sender, EventArgs e)
    {
        DynamicDataManager1.RegisterControl(DetailsView1);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        table = DetailsDataSource.GetTable();
        Title = table.DisplayName;

        ListHyperLink.NavigateUrl = table.ListActionPath;
    }

    protected void DetailsView1_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
    {
        if (e.Exception == null || e.ExceptionHandled)
        {
            Response.Redirect(table.ListActionPath);
        }
    }
}
