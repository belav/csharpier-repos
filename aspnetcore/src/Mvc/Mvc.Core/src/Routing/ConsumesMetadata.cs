using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    internal class ConsumesMetadata : IConsumesMetadata
    {
        public ConsumesMetadata(string[] contentTypes)
        {
            if (contentTypes == null)
            {
                throw new ArgumentNullException(nameof(contentTypes));
            }

            ContentTypes = contentTypes;
        }

        public IReadOnlyList<string> ContentTypes { get; }
    }
}
