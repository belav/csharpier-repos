using System.Buffers;

namespace Microsoft.AspNetCore.Components.RenderTree;

internal class TestArrayPool<T> : ArrayPool<T>
{
    public override T[] Rent(int minimumLength)
    {
        return new T[minimumLength];
    }

    public List<T[]> ReturnedBuffers = new List<T[]>();

    public override void Return(T[] array, bool clearArray = false)
    {
        ReturnedBuffers.Add(array);
    }
}
