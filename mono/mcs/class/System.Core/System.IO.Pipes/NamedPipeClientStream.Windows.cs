using System.Security.Principal;

namespace System.IO.Pipes
{
    partial public sealed class NamedPipeClientStream
    {
        int _access;

        public NamedPipeClientStream(
            string serverName,
            string pipeName,
            PipeAccessRights desiredAccessRights,
            PipeOptions options,
            TokenImpersonationLevel impersonationLevel,
            HandleInheritability inheritability
        )
            : this(
                serverName,
                pipeName,
                (PipeDirection)(
                    desiredAccessRights & (PipeAccessRights.ReadData | PipeAccessRights.WriteData)
                ),
                options,
                impersonationLevel,
                inheritability
            )
        {
            if (
                (
                    desiredAccessRights
                    & ~(PipeAccessRights.FullControl | PipeAccessRights.AccessSystemSecurity)
                ) != 0
            )
            {
                throw new ArgumentOutOfRangeException(
                    nameof(desiredAccessRights),
                    SR.ArgumentOutOfRange_InvalidPipeAccessRights
                );
            }

            // Referenced from CoreFX code
            _access = (int)desiredAccessRights;
        }
    }
}
