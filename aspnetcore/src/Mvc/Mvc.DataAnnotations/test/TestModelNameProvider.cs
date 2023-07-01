using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Microsoft.AspNetCore.Mvc.DataAnnotations;

public class TestModelNameProvider : IModelNameProvider
{
    public string Name { get; set; }
}
