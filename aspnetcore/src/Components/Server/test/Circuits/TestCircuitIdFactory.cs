using Microsoft.AspNetCore.DataProtection;

namespace Microsoft.AspNetCore.Components.Server.Circuits;

internal class TestCircuitIdFactory
{
    public static CircuitIdFactory CreateTestFactory()
    {
        return new CircuitIdFactory(new EphemeralDataProtectionProvider());
    }
}
