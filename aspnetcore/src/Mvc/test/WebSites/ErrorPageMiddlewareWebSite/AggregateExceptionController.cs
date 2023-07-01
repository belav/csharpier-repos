using Microsoft.AspNetCore.Mvc;

namespace ErrorPageMiddlewareWebSite;

public class AggregateExceptionController : Controller
{
    [HttpGet("/AggregateException")]
    public IActionResult Index()
    {
        var firstException = ThrowNullReferenceException();
        var secondException = ThrowIndexOutOfRangeException();
        Task.WaitAll(firstException, secondException);
        return View();
    }

    private static async Task ThrowNullReferenceException()
    {
        await Task.Delay(0);
        throw new NullReferenceException("Foo cannot be null");
    }

    private static async Task ThrowIndexOutOfRangeException()
    {
        await Task.Delay(0);
        throw new IndexOutOfRangeException("Index is out of range");
    }
}
