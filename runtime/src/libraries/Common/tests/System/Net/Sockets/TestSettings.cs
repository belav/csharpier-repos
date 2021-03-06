// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace System.Net.Sockets.Tests
{
    public static class TestSettings
    {
        // Timeout values in milliseconds.
        public const int PassingTestTimeout = 10_000;
        public const int PassingTestLongTimeout = 30_000;
        public const int FailingTestTimeout = 100;

        public static Task WhenAllOrAnyFailedWithTimeout(params Task[] tasks) => tasks.WhenAllOrAnyFailed(PassingTestTimeout);
    }
}
