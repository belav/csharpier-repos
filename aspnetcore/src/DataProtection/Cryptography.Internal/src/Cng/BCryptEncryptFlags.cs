using System;

namespace Microsoft.AspNetCore.Cryptography.Cng;

[Flags]
internal enum BCryptEncryptFlags
{
    BCRYPT_BLOCK_PADDING = 0x00000001,
}
