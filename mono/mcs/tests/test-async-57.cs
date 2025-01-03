using System;
using System.Threading.Tasks;

class X
{
    readonly Func<string, Task> action = null;

    public static void Main() { }

    protected async Task TestAsync()
    {
        await action("");
    }
}
