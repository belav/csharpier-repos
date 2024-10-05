using System;
using System.Collections;
using System.Collections.Specialized;
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

public partial class ForeignKeyField : System.Web.DynamicData.FieldTemplateUserControl
{
    private bool _allowNavigation = true;
    private string _navigateUrl;

    public string NavigateUrl
    {
        get { return _navigateUrl; }
        set { _navigateUrl = value; }
    }

    public bool AllowNavigation
    {
        get { return _allowNavigation; }
        set { _allowNavigation = value; }
    }

    protected string GetDisplayString()
    {
        return FormatFieldValue(ForeignKeyColumn.ParentTable.GetDisplayString(FieldValue));
    }

    protected string GetNavigateUrl()
    {
        if (!AllowNavigation)
        {
            return null;
        }

        if (String.IsNullOrEmpty(NavigateUrl))
        {
            return ForeignKeyPath;
        }
        else
        {
            return BuildForeignKeyPath(NavigateUrl);
        }
    }

    public override Control DataControl
    {
        get { return HyperLink1; }
    }
}
