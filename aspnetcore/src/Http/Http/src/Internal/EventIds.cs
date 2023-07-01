using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http
{
    internal static class EventIds
    {
        public static readonly EventId SameSiteNotSecure = new EventId(1, "SameSiteNotSecure");
    }
}
