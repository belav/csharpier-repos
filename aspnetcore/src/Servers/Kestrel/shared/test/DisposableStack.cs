using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Server.Kestrel.Tests;

public class DisposableStack<T> : Stack<T>, IDisposable
    where T : IDisposable
{
    public void Dispose()
    {
        while (Count > 0)
        {
            Pop()?.Dispose();
        }
    }
}
