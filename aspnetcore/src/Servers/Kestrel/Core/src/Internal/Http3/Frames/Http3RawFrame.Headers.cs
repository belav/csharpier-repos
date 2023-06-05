// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net.Http;

partial internal class Http3RawFrame
{
    public void PrepareHeaders()
    {
        Length = 0;
        Type = Http3FrameType.Headers;
    }
}
