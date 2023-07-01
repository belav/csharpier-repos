using Grpc.Core;

namespace InteropTestsClient;

/// <summary>
/// Extension methods that simplify work with gRPC streaming calls.
/// </summary>
public static class AsyncStreamExtensions
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

    /// <summary>
    /// Reads the entire stream and creates a list containing all the elements read.
    /// </summary>
    public static async Task<List<T>> ToListAsync<T>(this IAsyncStreamReader<T> streamReader)
        where T : class
    {
        var result = new List<T>();
        while (await streamReader.MoveNext().ConfigureAwait(false))
        {
            result.Add(streamReader.Current);
        }
        return result;
    }

    /// <summary>
    /// Writes all elements from given enumerable to the stream.
    /// Completes the stream afterwards unless close = false.
    /// </summary>
    public static async Task WriteAllAsync<T>(
        this IClientStreamWriter<T> streamWriter,
        IEnumerable<T> elements,
        bool complete = true
    )
        where T : class
    {
        foreach (var element in elements)
        {
            await streamWriter.WriteAsync(element).ConfigureAwait(false);
        }
        if (complete)
        {
            await streamWriter.CompleteAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Writes all elements from given enumerable to the stream.
    /// </summary>
    public static async Task WriteAllAsync<T>(
        this IServerStreamWriter<T> streamWriter,
        IEnumerable<T> elements
    )
        where T : class
    {
        foreach (var element in elements)
        {
            await streamWriter.WriteAsync(element).ConfigureAwait(false);
        }
    }
}
