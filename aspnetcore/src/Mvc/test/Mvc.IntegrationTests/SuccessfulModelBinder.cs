using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests;

/// <summary>
/// An unconditionally-successful model binder.
/// </summary>
public class SuccessfulModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var model = bindingContext.ModelType == typeof(bool) ? (object)true : null;
        bindingContext.Result = ModelBindingResult.Success(model);

        return Task.CompletedTask;
    }
}
