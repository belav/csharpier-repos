using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    internal interface IConsumesMetadata
    {
        IReadOnlyList<string> ContentTypes { get; }
    }
}
