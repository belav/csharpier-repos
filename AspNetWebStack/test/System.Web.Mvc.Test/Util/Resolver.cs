// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Web.Mvc.Test
{
    public class Resolver<T> : IResolver<T>
    {
        public T Current { get; set; }
    }
}
