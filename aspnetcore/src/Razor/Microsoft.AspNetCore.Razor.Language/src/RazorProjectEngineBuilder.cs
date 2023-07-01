using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language;

public abstract class RazorProjectEngineBuilder
{
    public abstract RazorConfiguration Configuration { get; }

    public abstract RazorProjectFileSystem FileSystem { get; }

    public abstract ICollection<IRazorFeature> Features { get; }

    public abstract IList<IRazorEnginePhase> Phases { get; }

    public abstract RazorProjectEngine Build();
}
