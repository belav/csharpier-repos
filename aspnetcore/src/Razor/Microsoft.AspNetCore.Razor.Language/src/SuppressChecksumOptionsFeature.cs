using System;

namespace Microsoft.AspNetCore.Razor.Language;

internal class SuppressChecksumOptionsFeature
    : RazorEngineFeatureBase,
        IConfigureRazorCodeGenerationOptionsFeature
{
    public int Order { get; set; }

    public void Configure(RazorCodeGenerationOptionsBuilder options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.SuppressChecksum = true;
    }
}
