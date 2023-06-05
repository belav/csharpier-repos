// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net
{
    partial internal sealed class ContextAwareResult
    {
        partial void SafeCaptureIdentity();

        partial void CleanupInternal();
    }
}
