// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Http;
using System.Net.Http.QPack;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http3;

internal abstract partial class Http3Stream : HttpProtocol, IHttp3Stream, IHttpHeadersHandler, IThreadPoolWorkItem
{
    private static ReadOnlySpan<byte> AuthorityBytes => new byte[10] { (byte)':', (byte)'a', (byte)'u', (byte)'t', (byte)'h', (byte)'o', (byte)'r', (byte)'i', (byte)'t', (byte)'y' };
    private static ReadOnlySpan<byte> MethodBytes => new byte[7] { (byte)':', (byte)'m', (byte)'e', (byte)'t', (byte)'h', (byte)'o', (byte)'d' };
    private static ReadOnlySpan<byte> PathBytes => new byte[5] { (byte)':', (byte)'p', (byte)'a', (byte)'t', (byte)'h' };
    private static ReadOnlySpan<byte> SchemeBytes => new byte[7] { (byte)':', (byte)'s', (byte)'c', (byte)'h', (byte)'e', (byte)'m', (byte)'e' };
    private static ReadOnlySpan<byte> StatusBytes => new byte[7] { (byte)':', (byte)'s', (byte)'t', (byte)'a', (byte)'t', (byte)'u', (byte)'s' };
    private static ReadOnlySpan<byte> ConnectionBytes => new byte[10] { (byte)'c', (byte)'o', (byte)'n', (byte)'n', (byte)'e', (byte)'c', (byte)'t', (byte)'i', (byte)'o', (byte)'n' };
    private static ReadOnlySpan<byte> TeBytes => new byte[2] { (byte)'t', (byte)'e' };
    private static ReadOnlySpan<byte> TrailersBytes => new byte[8] { (byte)'t', (byte)'r', (byte)'a', (byte)'i', (byte)'l', (byte)'e', (byte)'r', (byte)'s' };
    private static ReadOnlySpan<byte> ConnectBytes => new byte[7] { (byte)'C', (byte)'O', (byte)'N', (byte)'N', (byte)'E', (byte)'C', (byte)'T' };

    private const PseudoHeaderFields _mandatoryRequestPseudoHeaderFields =
        PseudoHeaderFields.Method | PseudoHeaderFields.Path | PseudoHeaderFields.Scheme;

    private Http3FrameWriter _frameWriter = default!;
    private Http3OutputProducer _http3Output = default!;
    private Http3StreamContext _context = default!;
    private IProtocolErrorCodeFeature _errorCodeFeature = default!;
    private IStreamIdFeature _streamIdFeature = default!;
    private IStreamAbortFeature _streamAbortFeature = default!;
    private int _isClosed;
    private readonly Http3RawFrame _incomingFrame = new Http3RawFrame();
    protected RequestHeaderParsingState _requestHeaderParsingState;
    private PseudoHeaderFields _parsedPseudoHeaderFields;
    private int _totalParsedHeaderSize;
    private bool _isMethodConnect;

    // TODO: Change to resetable ValueTask source
    private TaskCompletionSource? _appCompleted;

    private StreamCompletionFlags _completionState;
    private readonly object _completionLock = new object();

    public bool EndStreamReceived => (_completionState & StreamCompletionFlags.EndStreamReceived) == StreamCompletionFlags.EndStreamReceived;
    private bool IsAborted => (_completionState & StreamCompletionFlags.Aborted) == StreamCompletionFlags.Aborted;
    private bool IsCompleted => (_completionState & StreamCompletionFlags.Completed) == StreamCompletionFlags.Completed;

    public Pipe RequestBodyPipe { get; private set; } = default!;

    public long? InputRemaining { get; internal set; }

    public QPackDecoder QPackDecoder { get; private set; } = default!;

    public PipeReader Input => _context.Transport.Input;

    public ISystemClock SystemClock => _context.ServiceContext.SystemClock;
    public KestrelServerLimits Limits => _context.ServiceContext.ServerOptions.Limits;
    public long StreamId => _streamIdFeature.StreamId;

    public long StreamTimeoutTicks { get; set; }
    public bool IsReceivingHeader => _appCompleted == null; // TCS is assigned once headers are received
    public bool IsDraining => _appCompleted?.Task.IsCompleted ?? false; // Draining starts once app is complete

    public bool IsRequestStream => true;

    public void Initialize(Http3StreamContext context)
    {
        base.Initialize(context);

        InputRemaining = null;

        _context = context;

        _errorCodeFeature = _context.ConnectionFeatures.Get<IProtocolErrorCodeFeature>()!;
        _streamIdFeature = _context.ConnectionFeatures.Get<IStreamIdFeature>()!;
        _streamAbortFeature = _context.ConnectionFeatures.Get<IStreamAbortFeature>()!;

        _appCompleted = null;
        _isClosed = 0;
        _requestHeaderParsingState = default;
        _parsedPseudoHeaderFields = default;
        _totalParsedHeaderSize = 0;
        _isMethodConnect = false;
        _completionState = default;
        StreamTimeoutTicks = 0;

        if (_frameWriter == null)
        {
            _frameWriter = new Http3FrameWriter(
                context.StreamContext,
                context.TimeoutControl,
                context.ServiceContext.ServerOptions.Limits.MinResponseDataRate,
                context.MemoryPool,
                context.ServiceContext.Log,
                _streamIdFeature,
                context.ClientPeerSettings,
                this);

            _http3Output = new Http3OutputProducer(
                _frameWriter,
                context.MemoryPool,
                this,
                context.ServiceContext.Log);
            Output = _http3Output;
            RequestBodyPipe = CreateRequestBodyPipe(64 * 1024); // windowSize?
            QPackDecoder = new QPackDecoder(_context.ServiceContext.ServerOptions.Limits.Http3.MaxRequestHeaderFieldSize);
        }
        else
        {
            _http3Output.StreamReset();
            RequestBodyPipe.Reset();
            QPackDecoder.Reset();
        }

        _frameWriter.Reset(context.Transport.Output, context.ConnectionId);
    }

    public void InitializeWithExistingContext(IDuplexPipe transport)
    {
        _context.Transport = transport;
        Initialize(_context);
    }

    public void Abort(ConnectionAbortedException abortReason, Http3ErrorCode errorCode)
    {
        AbortCore(abortReason, errorCode);
    }

    private void AbortCore(Exception exception, Http3ErrorCode errorCode)
    {
        lock (_completionLock)
        {
            if (IsCompleted)
            {
                return;
            }

            var (oldState, newState) = ApplyCompletionFlag(StreamCompletionFlags.Aborted);

            if (oldState == newState)
            {
                return;
            }

            if (!(exception is ConnectionAbortedException abortReason))
            {
                abortReason = new ConnectionAbortedException(exception.Message, exception);
            }

            Log.Http3StreamAbort(TraceIdentifier, errorCode, abortReason);

            // Call _http3Output.Stop() prior to poisoning the request body stream or pipe to
            // ensure that an app that completes early due to the abort doesn't result in header frames being sent.
            _http3Output.Stop();

            CancelRequestAbortedToken();

            // Unblock the request body.
            PoisonBody(exception);
            RequestBodyPipe.Writer.Complete(exception);

            // Abort framewriter and underlying transport after stopping output.
            _errorCodeFeature.Error = (long)errorCode;
            _frameWriter.Abort(abortReason);
        }
    }

    protected override void OnErrorAfterResponseStarted()
    {
        // We can no longer change the response, send a Reset instead.
        var abortReason = new ConnectionAbortedException(CoreStrings.Http3StreamErrorAfterHeaders);
        Abort(abortReason, Http3ErrorCode.InternalError);
    }

    public void OnHeadersComplete(bool endStream)
    {
        OnHeadersComplete();
    }

    public void OnStaticIndexedHeader(int index)
    {
        var knownHeader = H3StaticTable.GetHeaderFieldAt(index);
        OnHeader(knownHeader.Name, knownHeader.Value);
    }

    public void OnStaticIndexedHeader(int index, ReadOnlySpan<byte> value)
    {
        var knownHeader = H3StaticTable.GetHeaderFieldAt(index);
        OnHeader(knownHeader.Name, value);
    }

    public void OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value) => OnHeader(name, value, checkForNewlineChars: true);

    public override void OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value, bool checkForNewlineChars)
    {
        // https://tools.ietf.org/html/rfc7540#section-6.5.2
        // "The value is based on the uncompressed size of header fields, including the length of the name and value in octets plus an overhead of 32 octets for each header field.";
        _totalParsedHeaderSize += HeaderField.RfcOverhead + name.Length + value.Length;
        if (_totalParsedHeaderSize > _context.ServiceContext.ServerOptions.Limits.MaxRequestHeadersTotalSize)
        {
            throw new Http3StreamErrorException(CoreStrings.BadRequest_HeadersExceedMaxTotalSize, Http3ErrorCode.RequestRejected);
        }

        ValidateHeader(name, value);
        try
        {
            if (_requestHeaderParsingState == RequestHeaderParsingState.Trailers)
            {
                OnTrailer(name, value);
            }
            else
            {
                // Throws BadRequest for header count limit breaches.
                // Throws InvalidOperation for bad encoding.
                base.OnHeader(name, value, checkForNewlineChars);
            }
        }
        catch (Microsoft.AspNetCore.Http.BadHttpRequestException bre)
        {
            throw new Http3StreamErrorException(bre.Message, Http3ErrorCode.MessageError);
        }
        catch (InvalidOperationException)
        {
            throw new Http3StreamErrorException(CoreStrings.BadRequest_MalformedRequestInvalidHeaders, Http3ErrorCode.MessageError);
        }
    }

    private void ValidateHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        // http://httpwg.org/specs/rfc7540.html#rfc.section.8.1.2.1
        /*
           Intermediaries that process HTTP requests or responses
           (i.e., any intermediary not acting as a tunnel) MUST NOT forward a
           malformed request or response. Malformed requests or responses that
           are detected MUST be treated as a stream error of type H3_MESSAGE_ERROR.

           For malformed requests, a server MAY send an HTTP response prior to
           closing or resetting the stream.  Clients MUST NOT accept a malformed
           response.  Note that these requirements are intended to protect
           against several types of common attacks against HTTP; they are
           deliberately strict because being permissive can expose
           implementations to these vulnerabilities.*/
        if (IsPseudoHeaderField(name, out var headerField))
        {
            if (_requestHeaderParsingState == RequestHeaderParsingState.Headers)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1-4
                // All pseudo-header fields MUST appear in the header block before regular header fields.
                // Any request or response that contains a pseudo-header field that appears in a header
                // block after a regular header field MUST be treated as malformed.
                throw new Http3StreamErrorException(CoreStrings.HttpErrorPseudoHeaderFieldAfterRegularHeaders, Http3ErrorCode.MessageError);
            }

            if (_requestHeaderParsingState == RequestHeaderParsingState.Trailers)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1-3
                // Pseudo-header fields MUST NOT appear in trailers.
                throw new Http3StreamErrorException(CoreStrings.HttpErrorTrailersContainPseudoHeaderField, Http3ErrorCode.MessageError);
            }

            _requestHeaderParsingState = RequestHeaderParsingState.PseudoHeaderFields;

            if (headerField == PseudoHeaderFields.Unknown)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1-3
                // Endpoints MUST treat a request or response that contains undefined or invalid pseudo-header
                // fields as malformed.
                throw new Http3StreamErrorException(CoreStrings.HttpErrorUnknownPseudoHeaderField, Http3ErrorCode.MessageError);
            }

            if (headerField == PseudoHeaderFields.Status)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1-3
                // Pseudo-header fields defined for requests MUST NOT appear in responses; pseudo-header fields
                // defined for responses MUST NOT appear in requests.
                throw new Http3StreamErrorException(CoreStrings.HttpErrorResponsePseudoHeaderField, Http3ErrorCode.MessageError);
            }

            if ((_parsedPseudoHeaderFields & headerField) == headerField)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1-7
                // All HTTP/3 requests MUST include exactly one valid value for the :method, :scheme, and :path pseudo-header fields
                throw new Http3StreamErrorException(CoreStrings.HttpErrorDuplicatePseudoHeaderField, Http3ErrorCode.MessageError);
            }

            if (headerField == PseudoHeaderFields.Method)
            {
                _isMethodConnect = value.SequenceEqual(ConnectBytes);
            }

            _parsedPseudoHeaderFields |= headerField;
        }
        else if (_requestHeaderParsingState != RequestHeaderParsingState.Trailers)
        {
            _requestHeaderParsingState = RequestHeaderParsingState.Headers;
        }

        if (IsConnectionSpecificHeaderField(name, value))
        {
            throw new Http3StreamErrorException(CoreStrings.HttpErrorConnectionSpecificHeaderField, Http3ErrorCode.MessageError);
        }

        // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1-3
        // A request or response containing uppercase header field names MUST be treated as malformed.
        for (var i = 0; i < name.Length; i++)
        {
            if (name[i] >= 65 && name[i] <= 90)
            {
                if (_requestHeaderParsingState == RequestHeaderParsingState.Trailers)
                {
                    throw new Http3StreamErrorException(CoreStrings.HttpErrorTrailerNameUppercase, Http3ErrorCode.MessageError);
                }
                else
                {
                    throw new Http3StreamErrorException(CoreStrings.HttpErrorHeaderNameUppercase, Http3ErrorCode.MessageError);
                }
            }
        }
    }

    private static bool IsPseudoHeaderField(ReadOnlySpan<byte> name, out PseudoHeaderFields headerField)
    {
        headerField = PseudoHeaderFields.None;

        if (name.IsEmpty || name[0] != (byte)':')
        {
            return false;
        }

        if (name.SequenceEqual(PathBytes))
        {
            headerField = PseudoHeaderFields.Path;
        }
        else if (name.SequenceEqual(MethodBytes))
        {
            headerField = PseudoHeaderFields.Method;
        }
        else if (name.SequenceEqual(SchemeBytes))
        {
            headerField = PseudoHeaderFields.Scheme;
        }
        else if (name.SequenceEqual(StatusBytes))
        {
            headerField = PseudoHeaderFields.Status;
        }
        else if (name.SequenceEqual(AuthorityBytes))
        {
            headerField = PseudoHeaderFields.Authority;
        }
        else
        {
            headerField = PseudoHeaderFields.Unknown;
        }

        return true;
    }

    private static bool IsConnectionSpecificHeaderField(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        return name.SequenceEqual(ConnectionBytes) || (name.SequenceEqual(TeBytes) && !value.SequenceEqual(TrailersBytes));
    }

    protected override void OnRequestProcessingEnded()
    {
        CompleteStream(errored: false);
    }

    private void CompleteStream(bool errored)
    {
        if (!EndStreamReceived)
        {
            if (!errored)
            {
                Log.RequestBodyNotEntirelyRead(ConnectionIdFeature, TraceIdentifier);
            }

            var (oldState, newState) = ApplyCompletionFlag(StreamCompletionFlags.AbortedRead);
            if (oldState != newState)
            {
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1-15
                // When the server does not need to receive the remainder of the request, it MAY abort reading
                // the request stream, send a complete response, and cleanly close the sending part of the stream.
                // The error code H3_NO_ERROR SHOULD be used when requesting that the client stop sending on the
                // request stream.
                _streamAbortFeature.AbortRead((long)Http3ErrorCode.NoError, new ConnectionAbortedException("The application completed without reading the entire request body."));
                RequestBodyPipe.Writer.Complete();
            }

            TryClose();
        }

        // Stream will be pooled after app completed.
        // Wait to signal app completed after any potential aborts on the stream.
        Debug.Assert(_appCompleted != null);
        _appCompleted.SetResult();
    }

    private bool TryClose()
    {
        if (Interlocked.Exchange(ref _isClosed, 1) == 0)
        {
            // Wake ProcessRequestAsync loop so that it can exit.
            Input.CancelPendingRead();

            return true;
        }

        return false;
    }

    public async Task ProcessRequestAsync<TContext>(IHttpApplication<TContext> application) where TContext : notnull
    {
        Exception? error = null;

        try
        {
            while (_isClosed == 0)
            {
                var result = await Input.ReadAsync();
                var readableBuffer = result.Buffer;
                var consumed = readableBuffer.Start;
                var examined = readableBuffer.End;

                try
                {
                    if (!readableBuffer.IsEmpty)
                    {
                        while (Http3FrameReader.TryReadFrame(ref readableBuffer, _incomingFrame, out var framePayload))
                        {
                            Log.Http3FrameReceived(ConnectionId, _streamIdFeature.StreamId, _incomingFrame);

                            consumed = examined = framePayload.End;
                            await ProcessHttp3Stream(application, framePayload, result.IsCompleted && readableBuffer.IsEmpty);
                        }
                    }

                    if (result.IsCompleted)
                    {
                        await OnEndStreamReceived();
                        return;
                    }
                }
                finally
                {
                    Input.AdvanceTo(consumed, examined);
                }
            }
        }
        catch (Http3StreamErrorException ex)
        {
            error = ex;
            Abort(new ConnectionAbortedException(ex.Message, ex), ex.ErrorCode);
        }
        catch (Http3ConnectionErrorException ex)
        {
            error = ex;
            _errorCodeFeature.Error = (long)ex.ErrorCode;

            _context.StreamLifetimeHandler.OnStreamConnectionError(ex);
        }
        catch (ConnectionAbortedException ex)
        {
            error = ex;
        }
        catch (ConnectionResetException ex)
        {
            error = ex;
            var resolvedErrorCode = _errorCodeFeature.Error >= 0 ? _errorCodeFeature.Error : 0;
            AbortCore(new IOException(CoreStrings.HttpStreamResetByClient, ex), (Http3ErrorCode)resolvedErrorCode);
        }
        catch (Exception ex)
        {
            error = ex;
            Log.LogWarning(0, ex, "Stream threw an unexpected exception.");
        }
        finally
        {
            var streamError = error as ConnectionAbortedException
                ?? new ConnectionAbortedException("The stream has completed.", error!);

            await Input.CompleteAsync();

            var appCompleted = _appCompleted?.Task ?? Task.CompletedTask;
            if (!appCompleted.IsCompletedSuccessfully)
            {
                // At this point in the stream's read-side is complete. However, with HTTP/3
                // the write-side of the stream can still be aborted by the client on request
                // aborted.
                //
                // To get notification of request aborted we register to connection closed
                // token. It will notify this type that the client has aborted the request
                // and Kestrel will complete pipes and cancel the RequestAborted token.
                //
                // Only subscribe to this event after the stream's read-side is complete to
                // avoid interactions between reading that is in-progress and an abort.
                // This means while reading, read-side abort will handle getting abort notifications.
                //
                // We don't need to hang on to the CancellationTokenRegistration from register.
                // The CTS is cleaned up in StreamContext.DisposeAsync.
                //
                // TODO: Consider a better way to provide this notification. For perf we want to
                // make the ConnectionClosed CTS pay-for-play, and change this event to use
                // something that is more lightweight than a CTS.
                _context.StreamContext.ConnectionClosed.Register(static s =>
                {
                    var stream = (Http3Stream)s!;

                    if (!stream.IsCompleted)
                    {
                            // An error code value other than -1 indicates a value was set and the request didn't gracefully complete.
                            var errorCode = stream._errorCodeFeature.Error;
                        if (errorCode >= 0)
                        {
                            stream.AbortCore(new IOException(CoreStrings.HttpStreamResetByClient), (Http3ErrorCode)errorCode);
                        }
                    }
                }, this);

                // Make sure application func is completed before completing writer.
                await appCompleted;
            }

            try
            {
                await _frameWriter.CompleteAsync();
            }
            catch
            {
                Abort(streamError, Http3ErrorCode.ProtocolError);
                throw;
            }
            finally
            {
                // Drain transports and dispose.
                await _context.StreamContext.DisposeAsync();

                // Tells the connection to remove the stream from its active collection.
                ApplyCompletionFlag(StreamCompletionFlags.Completed);
                _context.StreamLifetimeHandler.OnStreamCompleted(this);

                // TODO this is a hack for .NET 6 pooling.
                //
                // Pooling needs to happen after transports have been drained and stream
                // has been completed and is no longer active. All of this logic can't
                // be placed in ConnectionContext.DisposeAsync. Instead, QuicStreamContext
                // has pooling happen in QuicStreamContext.Dispose.
                //
                // ConnectionContext only implements IDisposableAsync by default. Only
                // QuicStreamContext should pass this check.
                if (_context.StreamContext is IDisposable disposableStream)
                {
                    disposableStream.Dispose();
                }
            }
        }
    }

    private ValueTask OnEndStreamReceived()
    {
        ApplyCompletionFlag(StreamCompletionFlags.EndStreamReceived);

        if (_requestHeaderParsingState == RequestHeaderParsingState.Ready)
        {
            // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1-14
            // Request stream ended without headers received. Unable to provide response.
            throw new Http3StreamErrorException(CoreStrings.Http3StreamErrorRequestEndedNoHeaders, Http3ErrorCode.RequestIncomplete);
        }

        if (InputRemaining.HasValue)
        {
            // https://tools.ietf.org/html/rfc7540#section-8.1.2.6
            if (InputRemaining.Value != 0)
            {
                throw new Http3StreamErrorException(CoreStrings.Http3StreamErrorLessDataThanLength, Http3ErrorCode.ProtocolError);
            }
        }

        OnTrailersComplete();
        return RequestBodyPipe.Writer.CompleteAsync();
    }

    private Task ProcessHttp3Stream<TContext>(IHttpApplication<TContext> application, in ReadOnlySequence<byte> payload, bool isCompleted) where TContext : notnull
    {
        switch (_incomingFrame.Type)
        {
            case Http3FrameType.Data:
                return ProcessDataFrameAsync(payload);
            case Http3FrameType.Headers:
                return ProcessHeadersFrameAsync(application, payload, isCompleted);
            case Http3FrameType.Settings:
            case Http3FrameType.CancelPush:
            case Http3FrameType.GoAway:
            case Http3FrameType.MaxPushId:
                // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-7.2.4
                // These frames need to be on a control stream
                throw new Http3ConnectionErrorException(CoreStrings.FormatHttp3ErrorUnsupportedFrameOnRequestStream(_incomingFrame.FormattedType), Http3ErrorCode.UnexpectedFrame);
            case Http3FrameType.PushPromise:
                // The server should never receive push promise
                throw new Http3ConnectionErrorException(CoreStrings.FormatHttp3ErrorUnsupportedFrameOnServer(_incomingFrame.FormattedType), Http3ErrorCode.UnexpectedFrame);
            default:
                return ProcessUnknownFrameAsync();
        }
    }

    private static Task ProcessUnknownFrameAsync()
    {
        // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-9
        // Unknown frames must be explicitly ignored.
        return Task.CompletedTask;
    }

    private async Task ProcessHeadersFrameAsync<TContext>(IHttpApplication<TContext> application, ReadOnlySequence<byte> payload, bool isCompleted) where TContext : notnull
    {
        // HEADERS frame after trailing headers is invalid.
        // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1
        if (_requestHeaderParsingState == RequestHeaderParsingState.Trailers)
        {
            throw new Http3ConnectionErrorException(CoreStrings.FormatHttp3StreamErrorFrameReceivedAfterTrailers(Http3Formatting.ToFormattedType(Http3FrameType.Headers)), Http3ErrorCode.UnexpectedFrame);
        }

        if (_requestHeaderParsingState == RequestHeaderParsingState.Headers)
        {
            _requestHeaderParsingState = RequestHeaderParsingState.Trailers;
        }

        try
        {
            QPackDecoder.Decode(payload, handler: this);
            QPackDecoder.Reset();
        }
        catch (QPackDecodingException ex)
        {
            Log.QPackDecodingError(ConnectionId, StreamId, ex);
            throw new Http3StreamErrorException(ex.Message, Http3ErrorCode.InternalError);
        }

        switch (_requestHeaderParsingState)
        {
            case RequestHeaderParsingState.Ready:
            case RequestHeaderParsingState.PseudoHeaderFields:
                _requestHeaderParsingState = RequestHeaderParsingState.Headers;
                break;
            case RequestHeaderParsingState.Headers:
                break;
            case RequestHeaderParsingState.Trailers:
                return;
            default:
                Debug.Fail("Unexpected header parsing state.");
                break;
        }

        InputRemaining = HttpRequestHeaders.ContentLength;

        // If the stream is complete after receiving the headers then run OnEndStreamReceived.
        // If there is a bad content length then this will throw before the request delegate is called.
        if (isCompleted)
        {
            await OnEndStreamReceived();
        }

        if (!_isMethodConnect && (_parsedPseudoHeaderFields & _mandatoryRequestPseudoHeaderFields) != _mandatoryRequestPseudoHeaderFields)
        {
            // All HTTP/3 requests MUST include exactly one valid value for the :method, :scheme, and :path pseudo-header
            // fields, unless it is a CONNECT request. An HTTP request that omits mandatory pseudo-header
            // fields is malformed.
            // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1.1.1
            throw new Http3StreamErrorException(CoreStrings.HttpErrorMissingMandatoryPseudoHeaderFields, Http3ErrorCode.MessageError);
        }

        _appCompleted = new TaskCompletionSource();
        StreamTimeoutTicks = default;
        _context.StreamLifetimeHandler.OnStreamHeaderReceived(this);

        ThreadPool.UnsafeQueueUserWorkItem(this, preferLocal: false);
    }

    private Task ProcessDataFrameAsync(in ReadOnlySequence<byte> payload)
    {
        // DATA frame before headers is invalid.
        // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1
        if (_requestHeaderParsingState == RequestHeaderParsingState.Ready)
        {
            throw new Http3ConnectionErrorException(CoreStrings.Http3StreamErrorDataReceivedBeforeHeaders, Http3ErrorCode.UnexpectedFrame);
        }

        // DATA frame after trailing headers is invalid.
        // https://quicwg.org/base-drafts/draft-ietf-quic-http.html#section-4.1
        if (_requestHeaderParsingState == RequestHeaderParsingState.Trailers)
        {
            var message = CoreStrings.FormatHttp3StreamErrorFrameReceivedAfterTrailers(Http3Formatting.ToFormattedType(Http3FrameType.Data));
            throw new Http3ConnectionErrorException(message, Http3ErrorCode.UnexpectedFrame);
        }

        if (InputRemaining.HasValue)
        {
            // https://tools.ietf.org/html/rfc7540#section-8.1.2.6
            if (payload.Length > InputRemaining.Value)
            {
                throw new Http3StreamErrorException(CoreStrings.Http3StreamErrorMoreDataThanLength, Http3ErrorCode.ProtocolError);
            }

            InputRemaining -= payload.Length;
        }

        foreach (var segment in payload)
        {
            RequestBodyPipe.Writer.Write(segment.Span);
        }

        // TODO this can be better.
        return RequestBodyPipe.Writer.FlushAsync().AsTask();
    }

    protected override void OnReset()
    {
        _keepAlive = true;
        _connectionAborted = false;
        _userTrailers = null;

        // Reset Http3 Features
        _currentIHttpMinRequestBodyDataRateFeature = this;
        _currentIHttpResponseTrailersFeature = this;
        _currentIHttpResetFeature = this;
    }

    protected override void ApplicationAbort() => ApplicationAbort(new ConnectionAbortedException(CoreStrings.ConnectionAbortedByApplication), Http3ErrorCode.InternalError);

    private void ApplicationAbort(ConnectionAbortedException abortReason, Http3ErrorCode error)
    {
        Abort(abortReason, error);
    }

    protected override string CreateRequestId()
    {
        return _context.StreamContext.ConnectionId;
    }

    protected override MessageBody CreateMessageBody()
        => Http3MessageBody.For(this);

    protected override bool TryParseRequest(ReadResult result, out bool endConnection)
    {
        endConnection = !TryValidatePseudoHeaders();
        return true;
    }

    private bool TryValidatePseudoHeaders()
    {
        _httpVersion = Http.HttpVersion.Http3;

        if (!TryValidateMethod())
        {
            return false;
        }

        if (!TryValidateAuthorityAndHost(out var hostText))
        {
            return false;
        }

        // CONNECT - :scheme and :path must be excluded
        if (Method == Http.HttpMethod.Connect)
        {
            if (!string.IsNullOrEmpty(RequestHeaders[HeaderNames.Scheme]) || !string.IsNullOrEmpty(RequestHeaders[HeaderNames.Path]))
            {
                Abort(new ConnectionAbortedException(CoreStrings.Http3ErrorConnectMustNotSendSchemeOrPath), Http3ErrorCode.ProtocolError);
                return false;
            }

            RawTarget = hostText;

            return true;
        }

        // :scheme https://tools.ietf.org/html/rfc7540#section-8.1.2.3
        // ":scheme" is not restricted to "http" and "https" schemed URIs.  A
        // proxy or gateway can translate requests for non - HTTP schemes,
        // enabling the use of HTTP to interact with non - HTTP services.
        var headerScheme = HttpRequestHeaders.HeaderScheme.ToString();
        HttpRequestHeaders.HeaderScheme = default; // Suppress pseduo headers from the public headers collection.
        if (!ReferenceEquals(headerScheme, Scheme) &&
            !string.Equals(headerScheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            if (!ServerOptions.AllowAlternateSchemes || !Uri.CheckSchemeName(headerScheme))
            {
                var str = CoreStrings.FormatHttp3StreamErrorSchemeMismatch(headerScheme, Scheme);
                Abort(new ConnectionAbortedException(str), Http3ErrorCode.ProtocolError);
                return false;
            }

            Scheme = headerScheme;
        }

        // :path (and query) - Required
        // Must start with / except may be * for OPTIONS
        var path = HttpRequestHeaders.HeaderPath.ToString();
        HttpRequestHeaders.HeaderPath = default; // Suppress pseduo headers from the public headers collection.
        RawTarget = path;

        // OPTIONS - https://tools.ietf.org/html/rfc7540#section-8.1.2.3
        // This pseudo-header field MUST NOT be empty for "http" or "https"
        // URIs; "http" or "https" URIs that do not contain a path component
        // MUST include a value of '/'.  The exception to this rule is an
        // OPTIONS request for an "http" or "https" URI that does not include
        // a path component; these MUST include a ":path" pseudo-header field
        // with a value of '*'.
        if (Method == Http.HttpMethod.Options && path.Length == 1 && path[0] == '*')
        {
            // * is stored in RawTarget only since HttpRequest expects Path to be empty or start with a /.
            Path = string.Empty;
            QueryString = string.Empty;
            return true;
        }

        // Approximate MaxRequestLineSize by totaling the required pseudo header field lengths.
        var requestLineLength = _methodText!.Length + Scheme!.Length + hostText.Length + path.Length;
        if (requestLineLength > ServerOptions.Limits.MaxRequestLineSize)
        {
            Abort(new ConnectionAbortedException(CoreStrings.BadRequest_RequestLineTooLong), Http3ErrorCode.RequestRejected);
            return false;
        }

        var queryIndex = path.IndexOf('?');
        QueryString = queryIndex == -1 ? string.Empty : path.Substring(queryIndex);

        var pathSegment = queryIndex == -1 ? path.AsSpan() : path.AsSpan(0, queryIndex);

        return TryValidatePath(pathSegment);
    }


    private bool TryValidateMethod()
    {
        // :method
        _methodText = HttpRequestHeaders.HeaderMethod.ToString();
        HttpRequestHeaders.HeaderMethod = default; // Suppress pseduo headers from the public headers collection.
        Method = HttpUtilities.GetKnownMethod(_methodText);

        if (Method == Http.HttpMethod.None)
        {
            Abort(new ConnectionAbortedException(CoreStrings.FormatHttp3ErrorMethodInvalid(_methodText)), Http3ErrorCode.ProtocolError);
            return false;
        }

        if (Method == Http.HttpMethod.Custom)
        {
            if (HttpCharacters.IndexOfInvalidTokenChar(_methodText) >= 0)
            {
                Abort(new ConnectionAbortedException(CoreStrings.FormatHttp3ErrorMethodInvalid(_methodText)), Http3ErrorCode.ProtocolError);
                return false;
            }
        }

        return true;
    }

    private bool TryValidateAuthorityAndHost(out string hostText)
    {
        // :authority (optional)
        // Prefer this over Host

        var authority = HttpRequestHeaders.HeaderAuthority;
        HttpRequestHeaders.HeaderAuthority = default; // Suppress pseduo headers from the public headers collection.
        var host = HttpRequestHeaders.HeaderHost;
        if (!StringValues.IsNullOrEmpty(authority))
        {
            // https://tools.ietf.org/html/rfc7540#section-8.1.2.3
            // Clients that generate HTTP/2 requests directly SHOULD use the ":authority"
            // pseudo - header field instead of the Host header field.
            // An intermediary that converts an HTTP/2 request to HTTP/1.1 MUST
            // create a Host header field if one is not present in a request by
            // copying the value of the ":authority" pseudo - header field.

            // We take this one step further, we don't want mismatched :authority
            // and Host headers, replace Host if :authority is defined. The application
            // will operate on the Host header.
            HttpRequestHeaders.HeaderHost = authority;
            host = authority;
        }

        // https://tools.ietf.org/html/rfc7230#section-5.4
        // A server MUST respond with a 400 (Bad Request) status code to any
        // HTTP/1.1 request message that lacks a Host header field and to any
        // request message that contains more than one Host header field or a
        // Host header field with an invalid field-value.
        hostText = host.ToString();
        if (host.Count > 1 || !HttpUtilities.IsHostHeaderValid(hostText))
        {
            // RST replaces 400
            Abort(new ConnectionAbortedException(CoreStrings.FormatBadRequest_InvalidHostHeader_Detail(hostText)), Http3ErrorCode.ProtocolError);
            return false;
        }

        return true;
    }

    [SkipLocalsInit]
    private bool TryValidatePath(ReadOnlySpan<char> pathSegment)
    {
        // Must start with a leading slash
        if (pathSegment.IsEmpty || pathSegment[0] != '/')
        {
            Abort(new ConnectionAbortedException(CoreStrings.FormatHttp3StreamErrorPathInvalid(RawTarget)), Http3ErrorCode.ProtocolError);
            return false;
        }

        var pathEncoded = pathSegment.Contains('%');

        // Compare with Http1Connection.OnOriginFormTarget

        // URIs are always encoded/escaped to ASCII https://tools.ietf.org/html/rfc3986#page-11
        // Multibyte Internationalized Resource Identifiers (IRIs) are first converted to utf8;
        // then encoded/escaped to ASCII  https://www.ietf.org/rfc/rfc3987.txt "Mapping of IRIs to URIs"

        try
        {
            const int MaxPathBufferStackAllocSize = 256;

            // The decoder operates only on raw bytes
            Span<byte> pathBuffer = pathSegment.Length <= MaxPathBufferStackAllocSize
                // A constant size plus slice generates better code
                // https://github.com/dotnet/aspnetcore/pull/19273#discussion_r383159929
                ? stackalloc byte[MaxPathBufferStackAllocSize].Slice(0, pathSegment.Length)
                // TODO - Consider pool here for less than 4096
                // https://github.com/dotnet/aspnetcore/pull/19273#discussion_r383604184
                : new byte[pathSegment.Length];

            for (var i = 0; i < pathSegment.Length; i++)
            {
                var ch = pathSegment[i];
                // The header parser should already be checking this
                Debug.Assert(32 < ch && ch < 127);
                pathBuffer[i] = (byte)ch;
            }

            Path = PathNormalizer.DecodePath(pathBuffer, pathEncoded, RawTarget!, QueryString!.Length);

            return true;
        }
        catch (InvalidOperationException)
        {
            Abort(new ConnectionAbortedException(CoreStrings.FormatHttp3StreamErrorPathInvalid(RawTarget)), Http3ErrorCode.ProtocolError);
            return false;
        }
    }

    private Pipe CreateRequestBodyPipe(uint windowSize)
        => new Pipe(new PipeOptions
        (
            pool: _context.MemoryPool,
            readerScheduler: ServiceContext.Scheduler,
            writerScheduler: PipeScheduler.Inline,
            // Never pause within the window range. Flow control will prevent more data from being added.
            // See the assert in OnDataAsync.
            pauseWriterThreshold: windowSize + 1,
            resumeWriterThreshold: windowSize + 1,
            useSynchronizationContext: false,
            minimumSegmentSize: _context.MemoryPool.GetMinimumSegmentSize()
        ));

    private (StreamCompletionFlags OldState, StreamCompletionFlags NewState) ApplyCompletionFlag(StreamCompletionFlags completionState)
    {
        lock (_completionLock)
        {
            var oldCompletionState = _completionState;
            _completionState |= completionState;

            return (oldCompletionState, _completionState);
        }
    }

    /// <summary>
    /// Used to kick off the request processing loop by derived classes.
    /// </summary>
    public abstract void Execute();

    protected enum RequestHeaderParsingState
    {
        Ready,
        PseudoHeaderFields,
        Headers,
        Trailers
    }

    [Flags]
    private enum PseudoHeaderFields
    {
        None = 0x0,
        Authority = 0x1,
        Method = 0x2,
        Path = 0x4,
        Scheme = 0x8,
        Status = 0x10,
        Unknown = 0x40000000
    }

    [Flags]
    private enum StreamCompletionFlags
    {
        None = 0,
        EndStreamReceived = 1,
        AbortedRead = 2,
        Aborted = 4,
        Completed = 8,
    }

    private static class GracefulCloseInitiator
    {
        public const int None = 0;
        public const int Server = 1;
        public const int Client = 2;
    }
}
