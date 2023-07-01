using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;

namespace InteropTestsWebsite;

// Implementation copied from https://github.com/grpc/grpc/blob/master/src/csharp/Grpc.Core/Utils/AsyncStreamExtensions.cs
internal static class AsyncStreamExtensions
{
    /// <summary>
    /// Reads the entire stream and executes an async action for each element.
    /// </summary>
    public static async Task ForEachAsync<T>(
        this IAsyncStreamReader<T> streamReader,
        Func<T, Task> asyncAction
    )
        where T : class
    {
        while (await streamReader.MoveNext().ConfigureAwait(false))
        {
            await asyncAction(streamReader.Current).ConfigureAwait(false);
        }
    }
}
