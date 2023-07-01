using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Razor.Language;

public class RazorProjectEngineFeatureBaseTest
{
    [Fact]
    public void ProjectEngineSetter_CallsOnInitialized()
    {
        // Arrange
        var testFeature = new TestFeature();

        // Act
        testFeature.ProjectEngine = Mock.Of<RazorProjectEngine>();

        // Assert
        Assert.Equal(1, testFeature.InitializationCount);
    }

    private class TestFeature : RazorProjectEngineFeatureBase
    {
        public int InitializationCount { get; private set; }

        protected override void OnInitialized()
        {
            InitializationCount++;
        }
    }
}
