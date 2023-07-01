using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Server.IntegrationTesting;

public class RuntimeArchitectures
{
    public static RuntimeArchitecture Current
    {
        get
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.Arm64 => RuntimeArchitecture.arm64,
                Architecture.X64 => RuntimeArchitecture.x64,
                Architecture.X86 => RuntimeArchitecture.x86,
                _
                    => throw new NotImplementedException(
                        $"Unknown RuntimeInformation.OSArchitecture: {RuntimeInformation.OSArchitecture.ToString()}"
                    ),
            };
        }
    }
}
