using System.Globalization;

namespace System.Net.Http
{
    internal static partial class SR
    {
        // The resource generator used in AspNetCore does not create this method. This file fills in that functional gap
        // so we don't have to modify the shared source.
        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                return string.Format(CultureInfo.CurrentCulture, resourceFormat, args);
            }

            return resourceFormat;
        }
    }
}
