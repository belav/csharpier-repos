// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http.HPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class Http2HeadersEnumeratorTests
    {
        [Fact]
        public void CanIterateOverResponseHeaders()
        {
            var responseHeaders = new HttpResponseHeaders
            {
                ContentLength = 9,
                HeaderAcceptRanges = "AcceptRanges!",
                HeaderAge = new StringValues(new[] { "1", "2" }),
                HeaderDate = "Date!",
                HeaderGrpcEncoding = "Identity!"
            };
            responseHeaders.Append("Name1", "Value1");
            responseHeaders.Append("Name2", "Value2-1");
            responseHeaders.Append("Name2", "Value2-2");
            responseHeaders.Append("Name3", "Value3");

            var e = new Http2HeadersEnumerator();
            e.Initialize(responseHeaders);

            var headers = GetNormalizedHeaders(e);

            Assert.Equal(new[]
            {
                CreateHeaderResult(H2StaticTable.Date, "Date", "Date!"),
                CreateHeaderResult(-1, "Grpc-Encoding", "Identity!"),
                CreateHeaderResult(H2StaticTable.AcceptRanges, "Accept-Ranges", "AcceptRanges!"),
                CreateHeaderResult(H2StaticTable.Age, "Age", "1"),
                CreateHeaderResult(H2StaticTable.Age, "Age", "2"),
                CreateHeaderResult(H2StaticTable.ContentLength, "Content-Length", "9"),
                CreateHeaderResult(-1, "Name1", "Value1"),
                CreateHeaderResult(-1, "Name2", "Value2-1"),
                CreateHeaderResult(-1, "Name2", "Value2-2"),
                CreateHeaderResult(-1, "Name3", "Value3"),
            }, headers);
        }

        [Fact]
        public void CanIterateOverResponseTrailers()
        {
            var responseTrailers = new HttpResponseTrailers
            {
                ContentLength = 9,
                HeaderETag = "ETag!"
            };
            responseTrailers.Append("Name1", "Value1");
            responseTrailers.Append("Name2", "Value2-1");
            responseTrailers.Append("Name2", "Value2-2");
            responseTrailers.Append("Name3", "Value3");

            var e = new Http2HeadersEnumerator();
            e.Initialize(responseTrailers);

            var headers = GetNormalizedHeaders(e);

            Assert.Equal(new[]
            {
                CreateHeaderResult(H2StaticTable.ETag, "ETag", "ETag!"),
                CreateHeaderResult(-1, "Name1", "Value1"),
                CreateHeaderResult(-1, "Name2", "Value2-1"),
                CreateHeaderResult(-1, "Name2", "Value2-2"),
                CreateHeaderResult(-1, "Name3", "Value3"),
            }, headers);
        }

        [Fact]
        public void Initialize_ChangeHeadersSource_EnumeratorUsesNewSource()
        {
            var responseHeaders = new HttpResponseHeaders();
            responseHeaders.Append("Name1", "Value1");
            responseHeaders.Append("Name2", "Value2-1");
            responseHeaders.Append("Name2", "Value2-2");

            var e = new Http2HeadersEnumerator();
            e.Initialize(responseHeaders);

            Assert.True(e.MoveNext());
            Assert.Equal("Name1", e.Current.Key);
            Assert.Equal("Value1", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.True(e.MoveNext());
            Assert.Equal("Name2", e.Current.Key);
            Assert.Equal("Value2-1", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.True(e.MoveNext());
            Assert.Equal("Name2", e.Current.Key);
            Assert.Equal("Value2-2", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            var responseTrailers = new HttpResponseTrailers
            {
                HeaderGrpcStatus = "1"
            };
            responseTrailers.Append("Name1", "Value1");
            responseTrailers.Append("Name2", "Value2-1");
            responseTrailers.Append("Name2", "Value2-2");

            e.Initialize(responseTrailers);

            Assert.True(e.MoveNext());
            Assert.Equal("Grpc-Status", e.Current.Key);
            Assert.Equal("1", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.True(e.MoveNext());
            Assert.Equal("Name1", e.Current.Key);
            Assert.Equal("Value1", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.True(e.MoveNext());
            Assert.Equal("Name2", e.Current.Key);
            Assert.Equal("Value2-1", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.True(e.MoveNext());
            Assert.Equal("Name2", e.Current.Key);
            Assert.Equal("Value2-2", e.Current.Value);
            Assert.Equal(-1, e.HPackStaticTableId);

            Assert.False(e.MoveNext());
        }

        private (int HPackStaticTableId, string Name, string Value)[] GetNormalizedHeaders(Http2HeadersEnumerator enumerator)
        {
            var headers = new List<(int HPackStaticTableId, string Name, string Value)>();
            while (enumerator.MoveNext())
            {
                headers.Add(CreateHeaderResult(enumerator.HPackStaticTableId, enumerator.Current.Key, enumerator.Current.Value));
            }
            return headers.ToArray();
        }

        private static (int HPackStaticTableId, string Key, string Value) CreateHeaderResult(int hPackStaticTableId, string key, string value)
        {
            return (hPackStaticTableId, key, value);
        }
    }
}
