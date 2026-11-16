using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MonoTests.DataSource;
using MonoTests.ModelProviders;
using MonoTests.System.Web.DynamicData;

namespace MonoTests.Common
{
    public interface ITestDataContext
    {
        IList GetTableData(
            string tableName,
            DataSourceSelectArguments args,
            string where,
            ParameterCollection whereParams
        );
        List<DynamicDataTable> GetTables();
    }
}
