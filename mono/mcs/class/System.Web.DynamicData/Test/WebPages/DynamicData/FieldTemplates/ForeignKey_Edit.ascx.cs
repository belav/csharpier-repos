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

public partial class ForeignKey_EditField : System.Web.DynamicData.FieldTemplateUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (DropDownList1.Items.Count == 0)
        {
            if (!Column.IsRequired)
            {
                DropDownList1.Items.Add(new ListItem("[Not Set]", ""));
            }

            PopulateListControl(DropDownList1);
        }
    }

    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);

        if (Mode == DataBoundControlMode.Edit)
        {
            string foreignkey = ForeignKeyColumn.GetForeignKeyString(Row);
            ListItem item = DropDownList1.Items.FindByValue(foreignkey);
            if (item != null)
            {
                DropDownList1.SelectedValue = foreignkey;
            }
        }
    }

    protected override void ExtractValues(IOrderedDictionary dictionary)
    {
        // If it's an empty string, change it to null
        string val = DropDownList1.SelectedValue;
        if (val == String.Empty)
            val = null;

        ExtractForeignKey(dictionary, val);
    }

    public override Control DataControl
    {
        get { return DropDownList1; }
    }
}
