using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate;

public sealed class MethodParameter
{
    public IList<string> Modifiers { get; } = new List<string>();

    public string TypeName { get; set; }

    public string ParameterName { get; set; }
}
