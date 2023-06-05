// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

#if FEATURE_PERFTRACING

namespace System.Diagnostics.Tracing
{
    partial internal static class EventPipeInternal
    {
        //
        // These PInvokes are used by the configuration APIs to interact with EventPipe.
        //
        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_Enable")]
        partial private static unsafe ulong Enable(
            char* outputFile,
            EventPipeSerializationFormat format,
            uint circularBufferSizeInMB,
            EventPipeProviderConfigurationNative* providers,
            uint numProviders
        );

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_Disable")]
        partial internal static void Disable(ulong sessionID);

        //
        // These PInvokes are used by EventSource to interact with the EventPipe.
        //
        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "EventPipeInternal_CreateProvider",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr CreateProvider(
            string providerName,
            Interop.Advapi32.EtwEnableCallback callbackFunc
        );

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_DefineEvent")]
        partial internal static unsafe IntPtr DefineEvent(
            IntPtr provHandle,
            uint eventID,
            long keywords,
            uint eventVersion,
            uint level,
            void* pMetadata,
            uint metadataLength
        );

        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "EventPipeInternal_GetProvider",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static IntPtr GetProvider(string providerName);

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_DeleteProvider")]
        partial internal static void DeleteProvider(IntPtr provHandle);

        [LibraryImport(
            RuntimeHelpers.QCall,
            EntryPoint = "EventPipeInternal_EventActivityIdControl"
        )]
        partial internal static int EventActivityIdControl(uint controlCode, ref Guid activityId);

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_WriteEventData")]
        partial internal static unsafe void WriteEventData(
            IntPtr eventHandle,
            EventProvider.EventData* pEventData,
            uint dataCount,
            Guid* activityId,
            Guid* relatedActivityId
        );

        //
        // These PInvokes are used as part of the EventPipeEventDispatcher.
        //
        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_GetSessionInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetSessionInfo(
            ulong sessionID,
            EventPipeSessionInfo* pSessionInfo
        );

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_GetNextEvent")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetNextEvent(
            ulong sessionID,
            EventPipeEventInstanceData* pInstance
        );

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_SignalSession")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool SignalSession(ulong sessionID);

        [LibraryImport(RuntimeHelpers.QCall, EntryPoint = "EventPipeInternal_WaitForSessionSignal")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool WaitForSessionSignal(ulong sessionID, int timeoutMs);
    }
}

#endif // FEATURE_PERFTRACING
