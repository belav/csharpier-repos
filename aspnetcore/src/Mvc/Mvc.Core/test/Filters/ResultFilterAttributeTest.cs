using Moq;

namespace Microsoft.AspNetCore.Mvc.Filters;

// These tests share code with the ActionFilterAttribute tests because the IAsyncResultFilter
// implementations need to behave the same way.
public class ResultFilterAttributeTest
{
    [Fact]
    public async Task ResultFilterAttribute_ResultFilter_Calls_OnResultExecuted()
    {
        await CommonFilterTest.ResultFilter_Calls_OnResultExecuted(
            new Mock<ResultFilterAttribute>()
        );
    }

    [Fact]
    public async Task ResultFilterAttribute_ResultFilter_SettingResult_DoesNotShortCircuit()
    {
        await CommonFilterTest.ResultFilter_SettingResult_DoesNotShortCircuit(
            new Mock<ResultFilterAttribute>()
        );
    }

    [Fact]
    public async Task ResultFilterAttribute_ResultFilter_SettingCancel_ShortCircuits()
    {
        await CommonFilterTest.ResultFilter_SettingCancel_ShortCircuits(
            new Mock<ResultFilterAttribute>()
        );
    }
}
