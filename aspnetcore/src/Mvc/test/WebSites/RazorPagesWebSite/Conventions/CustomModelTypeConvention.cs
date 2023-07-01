using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RazorPagesWebSite.Conventions;

internal class CustomModelTypeConvention : IPageApplicationModelConvention
{
    public void Apply(PageApplicationModel model)
    {
        if (model.ModelType == typeof(CustomModelTypeModel))
        {
            model.ModelType = typeof(CustomModelTypeModel<User>).GetTypeInfo();
        }
    }
}
