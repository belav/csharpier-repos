// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.AspNetCore.Internal;

public class CookieChunkingTests
{
    [Fact]
    public void AppendLargeCookie_Appended()
    {
        HttpContext context = new DefaultHttpContext();

        string testString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        new ChunkingCookieManager() { ChunkSize = null }.AppendResponseCookie(context, "TestCookie", testString, new CookieOptions());
        var values = context.Response.Headers["Set-Cookie"];
        Assert.Single(values);
        Assert.Equal("TestCookie=" + testString + "; path=/", values[0]);
    }

    [Fact]
    public void AppendLargeCookie_WithOptions_Appended()
    {
        HttpContext context = new DefaultHttpContext();
        var now = DateTimeOffset.UtcNow;
        var options = new CookieOptions
        {
            Domain = "foo.com",
            HttpOnly = true,
            SameSite = Http.SameSiteMode.Strict,
            Path = "/bar",
            Secure = true,
            Expires = now.AddMinutes(5),
            MaxAge = TimeSpan.FromMinutes(5)
        };
        var testString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        new ChunkingCookieManager() { ChunkSize = null }.AppendResponseCookie(context, "TestCookie", testString, options);

        var values = context.Response.Headers["Set-Cookie"];
        Assert.Single(values);
        Assert.Equal($"TestCookie={testString}; expires={now.AddMinutes(5).ToString("R")}; max-age=300; domain=foo.com; path=/bar; secure; samesite=strict; httponly", values[0]);
    }

    [Fact]
    public void AppendLargeCookieWithLimit_Chunked()
    {
        HttpContext context = new DefaultHttpContext();

        string testString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        new ChunkingCookieManager() { ChunkSize = 44 }.AppendResponseCookie(context, "TestCookie", testString, new CookieOptions());
        var values = context.Response.Headers["Set-Cookie"];
        Assert.Equal(4, values.Count);
        Assert.Equal<string[]>(new[]
        {
                "TestCookie=chunks-3; path=/",
                "TestCookieC1=abcdefghijklmnopqrstuv; path=/",
                "TestCookieC2=wxyz0123456789ABCDEFGH; path=/",
                "TestCookieC3=IJKLMNOPQRSTUVWXYZ; path=/",
            }, values);
    }

    [Fact]
    public void GetLargeChunkedCookie_Reassembled()
    {
        HttpContext context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = new[]
        {
                "TestCookie=chunks-7",
                "TestCookieC1=abcdefghi",
                "TestCookieC2=jklmnopqr",
                "TestCookieC3=stuvwxyz0",
                "TestCookieC4=123456789",
                "TestCookieC5=ABCDEFGHI",
                "TestCookieC6=JKLMNOPQR",
                "TestCookieC7=STUVWXYZ"
            };

        string result = new ChunkingCookieManager().GetRequestCookie(context, "TestCookie");
        string testString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Assert.Equal(testString, result);
    }

    [Fact]
    public void GetLargeChunkedCookieWithMissingChunk_ThrowingEnabled_Throws()
    {
        HttpContext context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = new[]
        {
                "TestCookie=chunks-7",
                "TestCookieC1=abcdefghi",
                // Missing chunk "TestCookieC2=jklmnopqr",
                "TestCookieC3=stuvwxyz0",
                "TestCookieC4=123456789",
                "TestCookieC5=ABCDEFGHI",
                "TestCookieC6=JKLMNOPQR",
                "TestCookieC7=STUVWXYZ"
            };

        Assert.Throws<FormatException>(() => new ChunkingCookieManager() { ThrowForPartialCookies = true }
            .GetRequestCookie(context, "TestCookie"));
    }

    [Fact]
    public void GetLargeChunkedCookieWithMissingChunk_ThrowingDisabled_NotReassembled()
    {
        HttpContext context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = new[]
        {
                "TestCookie=chunks-7",
                "TestCookieC1=abcdefghi",
                // Missing chunk "TestCookieC2=jklmnopqr",
                "TestCookieC3=stuvwxyz0",
                "TestCookieC4=123456789",
                "TestCookieC5=ABCDEFGHI",
                "TestCookieC6=JKLMNOPQR",
                "TestCookieC7=STUVWXYZ"
            };

        string result = new ChunkingCookieManager() { ThrowForPartialCookies = false }.GetRequestCookie(context, "TestCookie");
        string testString = "chunks-7";
        Assert.Equal(testString, result);
    }

    [Fact]
    public void DeleteChunkedCookieWithOptions_AllDeleted()
    {
        HttpContext context = new DefaultHttpContext();
        context.Request.Headers.Append("Cookie", "TestCookie=chunks-7");

        new ChunkingCookieManager().DeleteCookie(context, "TestCookie", new CookieOptions() { Domain = "foo.com", Secure = true });
        var cookies = context.Response.Headers["Set-Cookie"];
        Assert.Equal(8, cookies.Count);
        Assert.Equal(new[]
        {
                "TestCookie=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC1=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC2=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC3=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC4=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC5=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC6=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC7=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
            }, cookies);
    }



    [Fact]
    public void DeleteChunkedCookieWithOptionsAndResponseCookies_AllDeleted()
    {
        var chunkingCookieManager = new ChunkingCookieManager();
        HttpContext httpContext = new DefaultHttpContext();

        httpContext.Request.Headers["Cookie"] = new[]
        {
                "TestCookie=chunks-7",
                "TestCookieC1=abcdefghi",
                "TestCookieC2=jklmnopqr",
                "TestCookieC3=stuvwxyz0",
                "TestCookieC4=123456789",
                "TestCookieC5=ABCDEFGHI",
                "TestCookieC6=JKLMNOPQR",
                "TestCookieC7=STUVWXYZ"
            };

        var cookieOptions = new CookieOptions()
        {
            Domain = "foo.com",
            Path = "/",
            Secure = true
        };

        httpContext.Response.Headers[HeaderNames.SetCookie] = new[]
        {
                "TestCookie=chunks-7; domain=foo.com; path=/; secure",
                "TestCookieC1=STUVWXYZ; domain=foo.com; path=/; secure",
                "TestCookieC2=123456789; domain=foo.com; path=/; secure",
                "TestCookieC3=stuvwxyz0; domain=foo.com; path=/; secure",
                "TestCookieC4=123456789; domain=foo.com; path=/; secure",
                "TestCookieC5=ABCDEFGHI; domain=foo.com; path=/; secure",
                "TestCookieC6=JKLMNOPQR; domain=foo.com; path=/; secure",
                "TestCookieC7=STUVWXYZ; domain=foo.com; path=/; secure"
            };

        chunkingCookieManager.DeleteCookie(httpContext, "TestCookie", cookieOptions);
        Assert.Equal(8, httpContext.Response.Headers[HeaderNames.SetCookie].Count);
        Assert.Equal(new[]
        {
                "TestCookie=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC1=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC2=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC3=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC4=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC5=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC6=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure",
                "TestCookieC7=; expires=Thu, 01 Jan 1970 00:00:00 GMT; domain=foo.com; path=/; secure"
            }, httpContext.Response.Headers[HeaderNames.SetCookie]);
    }
}
