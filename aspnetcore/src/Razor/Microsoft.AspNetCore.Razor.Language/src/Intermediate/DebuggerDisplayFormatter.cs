using System.IO;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate;

internal class DebuggerDisplayFormatter : IntermediateNodeFormatterBase
{
    public DebuggerDisplayFormatter()
    {
        Writer = new StringWriter();
        ContentMode = FormatterContentMode.PreferContent;
    }

    public override string ToString()
    {
        return Writer.ToString();
    }
}
