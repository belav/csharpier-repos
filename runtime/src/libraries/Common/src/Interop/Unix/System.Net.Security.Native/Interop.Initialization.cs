// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial
    // Initialization of libssl threading support is done in a static constructor.
    // This enables a project simply to include this file, and any usage of any of
    // the Ssl functions will trigger initialization of the threading support.

    internal static class Ssl
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EnsureLibSslInitialized")]
        partial internal static void EnsureLibSslInitialized();

        static Ssl()
        {
            SslInitializer.Initialize();
        }
    }

    internal static class SslInitializer
    {
#if !SYSNETSECURITY_NO_OPENSSL
        static SslInitializer()
        {
            CryptoInitializer.Initialize();

            //Call ssl specific initializer
            Ssl.EnsureLibSslInitialized();
        }
#endif

        internal static void Initialize()
        {
            // No-op that exists to provide a hook for other static constructors
        }
    }
}
