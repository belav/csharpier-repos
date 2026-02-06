using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;

namespace System.Web.DynamicData
{
    /// <summary>
    /// Developers can use this interface to create their own filter repeaters.
    /// </summary>
    public interface IFilterExpressionProvider
    {
        IQueryable GetQueryable(IQueryable source);
        void Initialize(IQueryableDataSource dataSource);
    }
}
