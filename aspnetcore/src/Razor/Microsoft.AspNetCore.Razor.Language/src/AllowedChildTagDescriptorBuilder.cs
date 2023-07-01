using System;

namespace Microsoft.AspNetCore.Razor.Language;

public abstract class AllowedChildTagDescriptorBuilder
{
    public abstract string Name { get; set; }

    public abstract string DisplayName { get; set; }

    public abstract RazorDiagnosticCollection Diagnostics { get; }
}
