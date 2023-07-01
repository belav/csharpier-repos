using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Microsoft.AspNetCore.DataProtection;

internal sealed class RegistryPolicy
{
    public RegistryPolicy(
        AlgorithmConfiguration? configuration,
        IEnumerable<IKeyEscrowSink> keyEscrowSinks,
        int? defaultKeyLifetime
    )
    {
        EncryptorConfiguration = configuration;
        KeyEscrowSinks = keyEscrowSinks;
        DefaultKeyLifetime = defaultKeyLifetime;
    }

    public AlgorithmConfiguration? EncryptorConfiguration { get; }

    public IEnumerable<IKeyEscrowSink> KeyEscrowSinks { get; }

    public int? DefaultKeyLifetime { get; }
}
