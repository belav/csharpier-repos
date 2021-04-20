// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Server.IIS.Core.IO
{
    internal partial class WebSocketsAsyncIOEngine
    {
        internal class WebSocketReadOperation : AsyncIOOperation
        {
            [UnmanagedCallersOnly]
            public static NativeMethods.REQUEST_NOTIFICATION_STATUS ReadCallback(IntPtr httpContext, IntPtr completionInfo, IntPtr completionContext)
            {
                var context = (WebSocketReadOperation)GCHandle.FromIntPtr(completionContext).Target!;

                NativeMethods.HttpGetCompletionInfo(completionInfo, out var cbBytes, out var hr);

                var continuation = context.Complete(hr, cbBytes);

                continuation.Invoke();

                return NativeMethods.REQUEST_NOTIFICATION_STATUS.RQ_NOTIFICATION_PENDING;
            }

            private readonly WebSocketsAsyncIOEngine _engine;
            private GCHandle _thisHandle;
            private MemoryHandle _inputHandle;
            private NativeSafeHandle? _requestHandler;
            private Memory<byte> _memory;

            public WebSocketReadOperation(WebSocketsAsyncIOEngine engine)
            {
                _engine = engine;
            }

            protected override unsafe bool InvokeOperation(out int hr, out int bytes)
            {
                Debug.Assert(_requestHandler != null, "Must initialize first.");

                _thisHandle = GCHandle.Alloc(this);
                _inputHandle = _memory.Pin();

                hr = NativeMethods.HttpWebsocketsReadBytes(
                    _requestHandler,
                    (byte*)_inputHandle.Pointer,
                    _memory.Length,
                    &ReadCallback,
                    (IntPtr)_thisHandle,
                    out bytes,
                    out var completionExpected);

                return !completionExpected;
            }

            public void Initialize(NativeSafeHandle requestHandler, Memory<byte> memory)
            {
                _requestHandler = requestHandler;
                _memory = memory;
            }

            public override void FreeOperationResources(int hr, int bytes)
            {
                _inputHandle.Dispose();
            }

            protected override void ResetOperation()
            {
                base.ResetOperation();

                _thisHandle.Free();

                _memory = default;
                _inputHandle.Dispose();
                _inputHandle = default;
                _requestHandler = default;

                _engine.ReturnOperation(this);
            }

            protected override bool IsSuccessfulResult(int hr) => hr == NativeMethods.ERROR_HANDLE_EOF;
        }
    }
}
