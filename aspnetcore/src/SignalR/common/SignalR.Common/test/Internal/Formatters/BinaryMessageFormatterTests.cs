// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.SignalR.Internal;
using Xunit;

namespace Microsoft.AspNetCore.SignalR.Common.Tests.Internal.Formatters;

public partial class BinaryMessageFormatterTests
{
    [Fact]
    public void WriteMultipleMessages()
    {
        var expectedEncoding = new byte[]
        {
            /* length: */0x00,
            /* body: <empty> */
            /* length: */0x0E,
            /* body: */0x48,
            0x65,
            0x6C,
            0x6C,
            0x6F,
            0x2C,
            0x0D,
            0x0A,
            0x57,
            0x6F,
            0x72,
            0x6C,
            0x64,
            0x21,
        };

        var messages = new[] { new byte[0], Encoding.UTF8.GetBytes("Hello,\r\nWorld!") };

        var writer = MemoryBufferWriter.Get(); // Use small chunks to test Advance/Enlarge and partial payload writing
        try
        {
            foreach (var message in messages)
            {
                BinaryMessageFormatter.WriteLengthPrefix(message.Length, writer);
                writer.Write(message);
            }

            Assert.Equal(expectedEncoding, writer.ToArray());
        }
        finally
        {
            MemoryBufferWriter.Return(writer);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new byte[0])]
    [InlineData(
        new byte[]
        {
            0x80,
            0x01, // Size - 128
            0x00,
            0x01,
            0x02,
            0x03,
            0x04,
            0x05,
            0x06,
            0x07,
            0x08,
            0x09,
            0x0a,
            0x0b,
            0x0c,
            0x0d,
            0x0e,
            0x0f,
            0x10,
            0x11,
            0x12,
            0x13,
            0x14,
            0x15,
            0x16,
            0x17,
            0x18,
            0x19,
            0x1a,
            0x1b,
            0x1c,
            0x1d,
            0x1e,
            0x1f,
            0x20,
            0x21,
            0x22,
            0x23,
            0x24,
            0x25,
            0x26,
            0x27,
            0x28,
            0x29,
            0x2a,
            0x2b,
            0x2c,
            0x2d,
            0x2e,
            0x2f,
            0x30,
            0x31,
            0x32,
            0x33,
            0x34,
            0x35,
            0x36,
            0x37,
            0x38,
            0x39,
            0x3a,
            0x3b,
            0x3c,
            0x3d,
            0x3e,
            0x3f,
            0x40,
            0x41,
            0x42,
            0x43,
            0x44,
            0x45,
            0x46,
            0x47,
            0x48,
            0x49,
            0x4a,
            0x4b,
            0x4c,
            0x4d,
            0x4e,
            0x4f,
            0x50,
            0x51,
            0x52,
            0x53,
            0x54,
            0x55,
            0x56,
            0x57,
            0x58,
            0x59,
            0x5a,
            0x5b,
            0x5c,
            0x5d,
            0x5e,
            0x5f,
            0x60,
            0x61,
            0x62,
            0x63,
            0x64,
            0x65,
            0x66,
            0x67,
            0x68,
            0x69,
            0x6a,
            0x6b,
            0x6c,
            0x6d,
            0x6e,
            0x6f,
            0x70,
            0x71,
            0x72,
            0x73,
            0x74,
            0x75,
            0x76,
            0x77,
            0x78,
            0x79,
            0x7a,
            0x7b,
            0x7c,
            0x7d,
            0x7e,
            0x7f,
        },
        new byte[]
        {
            0x00,
            0x01,
            0x02,
            0x03,
            0x04,
            0x05,
            0x06,
            0x07,
            0x08,
            0x09,
            0x0a,
            0x0b,
            0x0c,
            0x0d,
            0x0e,
            0x0f,
            0x10,
            0x11,
            0x12,
            0x13,
            0x14,
            0x15,
            0x16,
            0x17,
            0x18,
            0x19,
            0x1a,
            0x1b,
            0x1c,
            0x1d,
            0x1e,
            0x1f,
            0x20,
            0x21,
            0x22,
            0x23,
            0x24,
            0x25,
            0x26,
            0x27,
            0x28,
            0x29,
            0x2a,
            0x2b,
            0x2c,
            0x2d,
            0x2e,
            0x2f,
            0x30,
            0x31,
            0x32,
            0x33,
            0x34,
            0x35,
            0x36,
            0x37,
            0x38,
            0x39,
            0x3a,
            0x3b,
            0x3c,
            0x3d,
            0x3e,
            0x3f,
            0x40,
            0x41,
            0x42,
            0x43,
            0x44,
            0x45,
            0x46,
            0x47,
            0x48,
            0x49,
            0x4a,
            0x4b,
            0x4c,
            0x4d,
            0x4e,
            0x4f,
            0x50,
            0x51,
            0x52,
            0x53,
            0x54,
            0x55,
            0x56,
            0x57,
            0x58,
            0x59,
            0x5a,
            0x5b,
            0x5c,
            0x5d,
            0x5e,
            0x5f,
            0x60,
            0x61,
            0x62,
            0x63,
            0x64,
            0x65,
            0x66,
            0x67,
            0x68,
            0x69,
            0x6a,
            0x6b,
            0x6c,
            0x6d,
            0x6e,
            0x6f,
            0x70,
            0x71,
            0x72,
            0x73,
            0x74,
            0x75,
            0x76,
            0x77,
            0x78,
            0x79,
            0x7a,
            0x7b,
            0x7c,
            0x7d,
            0x7e,
            0x7f,
        }
    )]
    [InlineData(new byte[] { 0x04, 0xAB, 0xCD, 0xEF, 0x12 }, new byte[] { 0xAB, 0xCD, 0xEF, 0x12 })]
    public void WriteBinaryMessage(byte[] encoded, byte[] payload)
    {
        var writer = MemoryBufferWriter.Get();
        try
        {
            BinaryMessageFormatter.WriteLengthPrefix(payload.Length, writer);
            writer.Write(payload);

            Assert.Equal(encoded, writer.ToArray());
        }
        finally
        {
            MemoryBufferWriter.Return(writer);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, "")]
    [InlineData(new byte[] { 0x03, 0x41, 0x42, 0x43 }, "ABC")]
    [InlineData(
        new byte[] { 0x0B, 0x41, 0x0A, 0x52, 0x0D, 0x43, 0x0D, 0x0A, 0x3B, 0x44, 0x45, 0x46 },
        "A\nR\rC\r\n;DEF"
    )]
    public void WriteTextMessage(byte[] encoded, string payload)
    {
        var message = Encoding.UTF8.GetBytes(payload);
        var writer = MemoryBufferWriter.Get();
        try
        {
            BinaryMessageFormatter.WriteLengthPrefix(message.Length, writer);
            writer.Write(message);

            Assert.Equal(encoded, writer.ToArray());
        }
        finally
        {
            MemoryBufferWriter.Return(writer);
        }
    }

    [Theory]
    [MemberData(nameof(RandomPayloads))]
    public void RoundTrippingTest(byte[] payload)
    {
        var writer = MemoryBufferWriter.Get();
        try
        {
            BinaryMessageFormatter.WriteLengthPrefix(payload.Length, writer);
            writer.Write(payload);
            var buffer = new ReadOnlySequence<byte>(writer.ToArray());
            Assert.True(BinaryMessageParser.TryParseMessage(ref buffer, out var roundtripped));
            Assert.Equal(payload, roundtripped.ToArray());
        }
        finally
        {
            MemoryBufferWriter.Return(writer);
        }
    }

    public static IEnumerable<object[]> RandomPayloads()
    {
        // boundaries
        yield return new[] { CreatePayload(0) };
        yield return new[] { CreatePayload(1) };
        yield return new[] { CreatePayload(0x7f) };
        yield return new[] { CreatePayload(0x80) };
        yield return new[] { CreatePayload(0x3fff) };
        yield return new[] { CreatePayload(0x4000) };

        // random
        yield return new[] { CreatePayload(0xc0de) };
    }

    private static byte[] CreatePayload(int size) =>
        Enumerable.Range(0, size).Select(n => (byte)(n & 0xff)).ToArray();
}
