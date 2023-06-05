// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.IsolatedStorage
{
    partial public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
    {
        private string GetIsolatedStorageRoot()
        {
            return Helper.GetRootDirectory(Scope);
        }
    }
}
