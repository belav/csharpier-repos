using System;

namespace Microsoft.AspNetCore.Razor.Language;

internal abstract class RazorHtmlDocument
{
    public abstract string GeneratedHtml { get; }

    public abstract RazorCodeGenerationOptions Options { get; }

    public static RazorHtmlDocument Create(string generatedHtml, RazorCodeGenerationOptions options)
    {
        if (generatedHtml == null)
        {
            throw new ArgumentNullException(nameof(generatedHtml));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return new DefaultRazorHtmlDocument(generatedHtml, options);
    }
}
