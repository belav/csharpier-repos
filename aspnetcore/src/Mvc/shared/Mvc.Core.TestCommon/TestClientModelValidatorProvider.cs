using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class TestClientModelValidatorProvider : CompositeClientModelValidatorProvider
{
    // Creates a provider with all the defaults - includes data annotations
    public static IClientModelValidatorProvider CreateDefaultProvider()
    {
        var providers = new IClientModelValidatorProvider[]
        {
            new DefaultClientModelValidatorProvider(),
            new DataAnnotationsClientModelValidatorProvider(
                new ValidationAttributeAdapterProvider(),
                Options.Create(new MvcDataAnnotationsLocalizationOptions()),
                stringLocalizerFactory: null
            ),
        };

        return new TestClientModelValidatorProvider(providers);
    }

    public TestClientModelValidatorProvider(IEnumerable<IClientModelValidatorProvider> providers)
        : base(providers) { }
}
