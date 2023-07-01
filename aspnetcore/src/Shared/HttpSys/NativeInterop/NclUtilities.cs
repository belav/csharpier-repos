using System;

namespace Microsoft.AspNetCore.HttpSys.Internal;

internal static class NclUtilities
{
    internal static bool HasShutdownStarted
    {
        get
        {
            return Environment.HasShutdownStarted
                || AppDomain.CurrentDomain.IsFinalizingForUnload();
        }
    }
}
