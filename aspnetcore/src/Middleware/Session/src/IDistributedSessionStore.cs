using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Session;

internal interface IDistributedSessionStore : IEnumerable<KeyValuePair<EncodedKey, byte[]>>
{
    int Count { get; }

    ICollection<EncodedKey> Keys { get; }

    bool TryGetValue(EncodedKey key, [MaybeNullWhen(false)] out byte[] value);

    void SetValue(EncodedKey key, byte[] value);

    bool Remove(EncodedKey encodedKey);

    void Clear();
}
