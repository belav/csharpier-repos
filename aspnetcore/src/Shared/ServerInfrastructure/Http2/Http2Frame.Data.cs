// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;

/*
    +---------------+
    |Pad Length? (8)|
    +---------------+-----------------------------------------------+
    |                            Data (*)                         ...
    +---------------------------------------------------------------+
    |                           Padding (*)                       ...
    +---------------------------------------------------------------+
*/
internal partial class Http2Frame
{
    public Http2DataFrameFlags DataFlags
    {
        get => (Http2DataFrameFlags)Flags;
        set => Flags = (byte)value;
    }

    public bool DataEndStream =>
        (DataFlags & Http2DataFrameFlags.END_STREAM) == Http2DataFrameFlags.END_STREAM;

    public bool DataHasPadding =>
        (DataFlags & Http2DataFrameFlags.PADDED) == Http2DataFrameFlags.PADDED;

    public byte DataPadLength { get; set; }

    private int DataPayloadOffset => DataHasPadding ? 1 : 0;

    public int DataPayloadLength => PayloadLength - DataPayloadOffset - DataPadLength;

    public void PrepareData(int streamId, byte? padLength = null)
    {
        PayloadLength = 0;
        Type = Http2FrameType.DATA;
        DataFlags = padLength.HasValue ? Http2DataFrameFlags.PADDED : Http2DataFrameFlags.NONE;
        StreamId = streamId;
        DataPadLength = padLength ?? 0;
    }
}
