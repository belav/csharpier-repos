using Microsoft.Extensions.ObjectPool;

namespace Microsoft.AspNetCore.Antiforgery;

internal sealed class AntiforgerySerializationContextPooledObjectPolicy
    : IPooledObjectPolicy<AntiforgerySerializationContext>
{
    public AntiforgerySerializationContext Create()
    {
        return new AntiforgerySerializationContext();
    }

    public bool Return(AntiforgerySerializationContext obj)
    {
        obj.Reset();

        return true;
    }
}
