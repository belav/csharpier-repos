// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HostedInAspNet.Server
{
    public class BootResourceRequestLog
    {
        private ConcurrentBag<string> _requestPaths = new ConcurrentBag<string>();

        public IReadOnlyCollection<string> RequestPaths => _requestPaths;

        public void AddRequest(HttpRequest request)
        {
            _requestPaths.Add(request.Path);
        }

        public void Clear()
        {
            _requestPaths.Clear();
        }
    }
}
