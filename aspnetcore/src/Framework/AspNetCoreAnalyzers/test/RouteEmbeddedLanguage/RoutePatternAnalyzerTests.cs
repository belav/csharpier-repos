// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Microsoft.AspNetCore.Analyzer.Testing;
using Microsoft.AspNetCore.Analyzers.RenderTreeBuilder;
using Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.Infrastructure;

namespace Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage;

public partial class RoutePatternAnalyzerTests
{
    private TestDiagnosticAnalyzerRunner Runner { get; } = new(new RoutePatternAnalyzer());

    [Fact]
    public async Task CommentOnString_ReportResults()
    {
        var source = TestSource.Read(
            @"
class Program
{
    static void Main()
    {
        // language=Route
        var s = @""/*MM*/~hi"";
    }
}"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_AttributeProperty_ReportResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;

class Program
{
    [HttpGet(Pattern = @""/*MM*/~hi"")]
    static void Main()
    {
    }
}

class HttpGet : Attribute
{
    [StringSyntax(""Route"")]
    public string Pattern { get; set; }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_AttributeCtorArgument_ReportResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;

class Program
{
    [HttpGet(@""/*MM*/~hi"")]
    static void Main()
    {
    }
}

class HttpGet : Attribute
{
    public HttpGet([StringSyntax(""Route"")] string pattern)
    {
    }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_FieldSet_ReportResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;

class Program
{
    static void Main()
    {
        field = @""/*MM*/~hi"";
    }

    [StringSyntax(""Route"")]
    private static string field;
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_PropertySet_ReportResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;

class Program
{
    static void Main()
    {
        prop = @""/*MM*/~hi"";
    }

    [StringSyntax(""Route"")]
    private static string prop { get; set; }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_MethodArgument_ReportResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;

class Program
{
    static void Main()
    {
        M(@""/*MM*/~hi"");
    }

    static void M([StringSyntax(""Route"")] string p)
    {
    }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Same(DiagnosticDescriptors.RoutePatternIssue, diagnostic.Descriptor);
        AnalyzerAssert.DiagnosticLocation(source.DefaultMarkerLocation, diagnostic.Location);
        Assert.Equal(
            Resources.FormatAnalyzer_RouteIssue_Message(
                Resources.TemplateRoute_InvalidRouteTemplate
            ),
            diagnostic.GetMessage(CultureInfo.InvariantCulture)
        );
    }

    [Fact]
    public async Task StringSyntax_MethodArgument_MultipleResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;

class Program
{
    static void Main()
    {
        M(@""~hi?"");
    }

    static void M([StringSyntax(""Route"")] string p)
    {
    }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Collection(
            diagnostics,
            d =>
            {
                Assert.Same(DiagnosticDescriptors.RoutePatternIssue, d.Descriptor);
                Assert.Equal(
                    Resources.FormatAnalyzer_RouteIssue_Message(
                        Resources.FormatTemplateRoute_InvalidLiteral("~hi?")
                    ),
                    d.GetMessage(CultureInfo.InvariantCulture)
                );
            },
            d =>
            {
                Assert.Same(DiagnosticDescriptors.RoutePatternIssue, d.Descriptor);
                Assert.Equal(
                    Resources.FormatAnalyzer_RouteIssue_Message(
                        Resources.TemplateRoute_InvalidRouteTemplate
                    ),
                    d.GetMessage(CultureInfo.InvariantCulture)
                );
            }
        );
    }

    [Fact]
    public async Task BadTokenReplacement_MethodArgument_MultipleResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;

class Program
{
    static void Main()
    {
        M(@""[hi"");
    }

    static void M([StringSyntax(""Route"")] string p)
    {
    }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task BadTokenReplacement_MvcAction_TokenReplacementDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
    }
}

[Route(@""[hi"")]
public class TestController
{
    public void TestAction()
    {
    }
}
"
        );
        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Collection(
            diagnostics,
            d =>
            {
                Assert.Same(DiagnosticDescriptors.RoutePatternIssue, d.Descriptor);
                Assert.Equal(
                    Resources.FormatAnalyzer_RouteIssue_Message(
                        Resources.AttributeRoute_TokenReplacement_UnclosedToken
                    ),
                    d.GetMessage(CultureInfo.InvariantCulture)
                );
            }
        );
    }

    [Fact]
    public async Task ControllerAction_UnusedRouteParameter_ReportedDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
    }
}

public class TestController
{
    [HttpGet(@""{id}"")]
    public object TestAction()
    {
        return null;
    }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Collection(
            diagnostics,
            d =>
            {
                Assert.Same(DiagnosticDescriptors.RoutePatternUnusedParameter, d.Descriptor);
                Assert.Equal(
                    Resources.FormatAnalyzer_UnusedParameter_Message("id"),
                    d.GetMessage(CultureInfo.InvariantCulture)
                );
            }
        );
    }

    [Fact]
    public async Task ControllerAction_MatchRouteParameterWithFromRoute_NoDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
    }
}

public class TestController
{
    [HttpGet(@""{id}"")]
    public object TestAction([FromRoute(Name = ""id"")]string id1)
    {
        return null;
    }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task ControllerAction_MatchRouteParameterWithMultipleFromRoute_NoDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
    }
}

public class TestController
{
    [HttpGet(@""{id}"")]
    public object TestAction([CustomFromRoute(Name = ""id"")][FromRoute(Name = ""custom_id"")]string id1)
    {
        return null;
    }
}

public class CustomFromRouteAttribute : Attribute, IFromRouteMetadata
{
    public string? Name { get; set; }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task MapGet_AsParameter_NoResults()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

class Program
{
    static void Main()
    {
        EndpointRouteBuilderExtensions.MapGet(null, @""{PageNumber}/{PageIndex}"", ([AsParameters] PageData pageData) => """");
    }

    int OtherMethod(PageData pageData)
    {
        return pageData.PageIndex;
    }
}

public class PageData
{
    public int PageNumber { get; set; }
    public int PageIndex { get; set; }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact]
    public async Task MapGet_AsParameter_Extra_ReportedDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

class Program
{
    static void Main()
    {
        EndpointRouteBuilderExtensions.MapGet(null, @""{PageNumber}/{PageIndex}/{id}"", ([AsParameters] PageData pageData) => """");
    }

    int OtherMethod(PageData pageData)
    {
        return pageData.PageIndex;
    }
}

public class PageData
{
    public int PageNumber { get; set; }
    public int PageIndex { get; set; }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Collection(
            diagnostics,
            d =>
            {
                Assert.Same(DiagnosticDescriptors.RoutePatternUnusedParameter, d.Descriptor);
                Assert.Equal(
                    Resources.FormatAnalyzer_UnusedParameter_Message("id"),
                    d.GetMessage(CultureInfo.InvariantCulture)
                );
            }
        );
    }

    [Fact]
    public async Task ControllerAction_MatchedRouteParameter_NoDiagnostics()
    {
        // Arrange
        var source = TestSource.Read(
            @"
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

class Program
{
    static void Main()
    {
    }
}

public class TestController
{
    [HttpGet(@""{id}"")]
    public object TestAction(int id)
    {
        return null;
    }
}
"
        );

        // Act
        var diagnostics = await Runner.GetDiagnosticsAsync(source.Source);

        // Assert
        Assert.Empty(diagnostics);
    }
}
