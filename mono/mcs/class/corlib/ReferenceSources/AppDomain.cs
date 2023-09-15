using System.Reflection;
using System.Runtime.Versioning;
using System.Security;

namespace System
{
    public partial class AppDomain
    {
        internal static bool IsAppXModel()
        {
            return false;
        }

        internal static bool IsAppXDesignMode()
        {
            return false;
        }

        internal static void CheckReflectionOnlyLoadSupported() { }

        internal static void CheckLoadFromSupported() { }
    }
}
