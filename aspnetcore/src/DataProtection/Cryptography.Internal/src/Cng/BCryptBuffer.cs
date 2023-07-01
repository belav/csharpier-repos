using System;
using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Cryptography.Cng;

// http://msdn.microsoft.com/en-us/library/windows/desktop/aa375368(v=vs.85).aspx
[StructLayout(LayoutKind.Sequential)]
internal struct BCryptBuffer
{
    public uint cbBuffer; // Length of buffer, in bytes
    public BCryptKeyDerivationBufferType BufferType; // Buffer type
    public IntPtr pvBuffer; // Pointer to buffer
}
