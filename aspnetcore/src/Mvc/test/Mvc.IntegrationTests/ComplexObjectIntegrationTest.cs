using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests;

public class ComplexObjectIntegrationTest : ComplexTypeIntegrationTestBase
{
    protected override Type ExpectedModelBinderType => typeof(ComplexObjectModelBinder);
}
