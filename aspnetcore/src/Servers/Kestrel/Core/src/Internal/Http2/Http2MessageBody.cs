// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2
{
    internal sealed class Http2MessageBody : MessageBody
    {
        private readonly Http2Stream _context;
        private ReadResult _readResult;

        public Http2MessageBody(Http2Stream context)
            : base(context)
        {
            _context = context;
        }

        protected override void OnReadStarting()
        {
            // Note ContentLength or MaxRequestBodySize may be null
            if (_context.RequestHeaders.ContentLength > _context.MaxRequestBodySize)
            {
                KestrelBadHttpRequestException.Throw(RequestRejectionReason.RequestBodyTooLarge);
            }
        }

        protected override Task OnReadStartedAsync()
        {
            // Produce 100-continue if no request body data for the stream has arrived yet.
            if (!_context.RequestBodyStarted)
            {
                ValueTask<FlushResult> continueTask = TryProduceContinueAsync();
                if (!continueTask.IsCompletedSuccessfully)
                {
                    return continueTask.GetAsTask();
                }
            }

            return Task.CompletedTask;
        }

        public override void Reset()
        {
            base.Reset();
            _readResult = default;
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            var newlyExaminedBytes = TrackConsumedAndExaminedBytes(_readResult, consumed, examined);

            // Ensure we consume data from the RequestBodyPipe before sending WINDOW_UPDATES to the client.
            _context.RequestBodyPipe.Reader.AdvanceTo(consumed, examined);

            // The HTTP/2 flow control window cannot be larger than 2^31-1 which limits bytesRead.
            _context.OnDataRead((int)newlyExaminedBytes);
            AddAndCheckObservedBytes(newlyExaminedBytes);
        }

        public override bool TryRead(out ReadResult readResult)
        {
            TryStartAsync();

            var hasResult = _context.RequestBodyPipe.Reader.TryRead(out readResult);

            if (hasResult)
            {
                _readResult = readResult;

                CountBytesRead(readResult.Buffer.Length);

                if (readResult.IsCompleted)
                {
                    TryStop();
                }
            }

            return hasResult;
        }

        public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            await TryStartAsync();

            try
            {
                var readAwaitable = _context.RequestBodyPipe.Reader.ReadAsync(cancellationToken);

                _readResult = await StartTimingReadAsync(readAwaitable, cancellationToken);
            }
            catch (ConnectionAbortedException ex)
            {
                throw new TaskCanceledException("The request was aborted", ex);
            }

            StopTimingRead(_readResult.Buffer.Length);

            if (_readResult.IsCompleted)
            {
                TryStop();
            }

            return _readResult;
        }

        public override void Complete(Exception? exception)
        {
            _context.ReportApplicationError(exception);
            _context.RequestBodyPipe.Reader.Complete();
        }

        public override ValueTask CompleteAsync(Exception? exception)
        {
            _context.ReportApplicationError(exception);
            return _context.RequestBodyPipe.Reader.CompleteAsync();
        }

        public override void CancelPendingRead()
        {
            _context.RequestBodyPipe.Reader.CancelPendingRead();
        }

        protected override ValueTask OnStopAsync()
        {
            if (!_context.HasStartedConsumingRequestBody)
            {
                return default;
            }

            _context.RequestBodyPipe.Reader.Complete();

            return default;
        }
    }
}
