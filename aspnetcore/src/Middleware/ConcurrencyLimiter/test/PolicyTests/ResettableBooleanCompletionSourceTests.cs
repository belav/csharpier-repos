// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.ConcurrencyLimiter.Tests.PolicyTests;

public static class ResettableBooleanCompletionSourceTests
{
    private static readonly StackPolicy _testQueue = TestUtils.CreateStackPolicy(8);

    [Fact]
    public static async Task CanBeAwaitedMultipleTimes()
    {
        var tcs = new ResettableBooleanCompletionSource(_testQueue);

        tcs.Complete(true);
        Assert.True(await tcs.GetValueTask());

        tcs.Complete(true);
        Assert.True(await tcs.GetValueTask());

        tcs.Complete(false);
        Assert.False(await tcs.GetValueTask());

        tcs.Complete(false);
        Assert.False(await tcs.GetValueTask());
    }

    [Fact]
    public static async Task CanSetResultToTrue()
    {
        var tcs = new ResettableBooleanCompletionSource(_testQueue);

        _ = Task.Run(() =>
        {
            tcs.Complete(true);
        });

        var result = await tcs.GetValueTask();
        Assert.True(result);
    }

    [Fact]
    public static async Task CanSetResultToFalse()
    {
        var tcs = new ResettableBooleanCompletionSource(_testQueue);

        _ = Task.Run(() =>
        {
            tcs.Complete(false);
        });

        var result = await tcs.GetValueTask();
        Assert.False(result);
    }

    [Fact]
    public static void DoubleCallToGetResultCausesError()
    {
        // important to verify it throws rather than acting like a new task

        var tcs = new ResettableBooleanCompletionSource(_testQueue);
        var task = tcs.GetValueTask();
        tcs.Complete(true);

        Assert.True(task.Result);
        Assert.Throws<InvalidOperationException>(() => task.Result);
    }

    [Fact]
    public static Task RunsContinuationsAsynchronously()
    {
        var tcs = new TaskCompletionSource<object>();

        async void RunTest()
        {
            try
            {
                await RunsContinuationsAsynchronouslyInternally();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                throw;
            }

            tcs.SetResult(null);
        }

        // The Xunit TestSyncContext causes the resettable tcs to always dispatch in effect.
        ThreadPool.UnsafeQueueUserWorkItem(_ => RunTest(), state: null);

        return tcs.Task;
    }

    private static async Task RunsContinuationsAsynchronouslyInternally()
    {
        var tcs = new ResettableBooleanCompletionSource(_testQueue);
        var mre = new ManualResetEventSlim();

        async Task AwaitAndBlock()
        {
            await tcs.GetValueTask();
            mre.Wait();
        }

        var task = AwaitAndBlock();

        await Task.Run(() => tcs.Complete(true)).DefaultTimeout();

        Assert.False(task.IsCompleted);

        mre.Set();
        await task.DefaultTimeout();
    }
}
