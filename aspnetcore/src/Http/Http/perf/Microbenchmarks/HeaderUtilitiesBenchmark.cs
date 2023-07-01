using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Net.Http.Headers;

public class HeaderUtilitiesBenchmark
{
    [Benchmark]
    public StringSegment UnescapeAsQuotedString()
    {
        return HeaderUtilities.UnescapeAsQuotedString("\"hello\\\"foo\\\\bar\\\\baz\\\\\"");
    }

    [Benchmark]
    public StringSegment EscapeAsQuotedString()
    {
        return HeaderUtilities.EscapeAsQuotedString("\"hello\\\"foo\\\\bar\\\\baz\\\\\"");
    }
}
