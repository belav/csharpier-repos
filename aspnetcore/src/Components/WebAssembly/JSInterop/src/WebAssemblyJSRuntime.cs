// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.JSInterop.Infrastructure;
using WebAssembly.JSInterop;

namespace Microsoft.JSInterop.WebAssembly;

/// <summary>
/// Provides methods for invoking JavaScript functions for applications running
/// on the Mono WebAssembly runtime.
/// </summary>
public abstract class WebAssemblyJSRuntime : JSInProcessRuntime, IJSUnmarshalledRuntime
{
    /// <summary>
    /// Initializes a new instance of <see cref="WebAssemblyJSRuntime"/>.
    /// </summary>
    protected WebAssemblyJSRuntime()
    {
        JsonSerializerOptions.Converters.Insert(0, new WebAssemblyJSObjectReferenceJsonConverter(this));
    }

    /// <inheritdoc />
    protected override string InvokeJS(string identifier, [StringSyntax(StringSyntaxAttribute.Json)] string? argsJson, JSCallResultType resultType, long targetInstanceId)
    {
        try
        {
            return InternalCalls.InvokeJSJson(identifier, targetInstanceId, (int)resultType, argsJson ?? "[]", 0);
        }
        catch (Exception ex)
        {
            throw new JSException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    protected override void BeginInvokeJS(long asyncHandle, string identifier, [StringSyntax(StringSyntaxAttribute.Json)] string? argsJson, JSCallResultType resultType, long targetInstanceId)
    {
        InternalCalls.InvokeJSJson(identifier, targetInstanceId, (int)resultType, argsJson ?? "[]", asyncHandle);
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "TODO: This should be in the xml suppressions file, but can't be because https://github.com/mono/linker/issues/2006")]
    protected override void EndInvokeDotNet(DotNetInvocationInfo callInfo, in DotNetInvocationResult dispatchResult)
    {
        var resultJsonOrErrorMessage = dispatchResult.Success
            ? dispatchResult.ResultJson!
            : dispatchResult.Exception!.ToString();
        InternalCalls.EndInvokeDotNetFromJS(callInfo.CallId, dispatchResult.Success, resultJsonOrErrorMessage);
    }

    /// <inheritdoc />
    protected override void SendByteArray(int id, byte[] data)
    {
        InternalCalls.ReceiveByteArray(id, data);
    }

    [Obsolete("This method is obsolete. Use JSImportAttribute instead.")]
    internal TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, long targetInstanceId)
    {
        var resultType = JSCallResultTypeHelper.FromGeneric<TResult>();

        var callInfo = new JSCallInfo
        {
            FunctionIdentifier = identifier,
            TargetInstanceId = targetInstanceId,
            ResultType = resultType,
        };

        string exception;

        switch (resultType)
        {
            case JSCallResultType.Default:
            case JSCallResultType.JSVoidResult:
                var result = InternalCalls.InvokeJS<T0, T1, T2, TResult>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : result;
            case JSCallResultType.JSObjectReference:
                var id = InternalCalls.InvokeJS<T0, T1, T2, int>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : (TResult)(object)new WebAssemblyJSObjectReference(this, id);
            case JSCallResultType.JSStreamReference:
                var serializedStreamReference = InternalCalls.InvokeJS<T0, T1, T2, string>(out exception, ref callInfo, arg0, arg1, arg2);
                return exception != null
                    ? throw new JSException(exception)
                    : (TResult)(object)DeserializeJSStreamReference(serializedStreamReference);
            default:
                throw new InvalidOperationException($"Invalid result type '{resultType}'.");
        }
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "IJSStreamReference is referenced in Microsoft.JSInterop.Infrastructure.JSStreamReferenceJsonConverter")]
    private IJSStreamReference DeserializeJSStreamReference(string serializedStreamReference)
    {
        var jsStreamReference = JsonSerializer.Deserialize<IJSStreamReference>(serializedStreamReference, JsonSerializerOptions);
        if (jsStreamReference is null)
        {
            throw new ArgumentException($"Failed to parse as {nameof(IJSStreamReference)}.", nameof(serializedStreamReference));
        }

        return jsStreamReference;
    }

    /// <inheritdoc />
    [Obsolete("This method is obsolete. Use JSImportAttribute instead.")]
    public TResult InvokeUnmarshalled<TResult>(string identifier)
        => InvokeUnmarshalled<object?, object?, object?, TResult>(identifier, null, null, null, 0);

    /// <inheritdoc />
    [Obsolete("This method is obsolete. Use JSImportAttribute instead.")]
    public TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0)
        => InvokeUnmarshalled<T0, object?, object?, TResult>(identifier, arg0, null, null, 0);

    /// <inheritdoc />
    [Obsolete("This method is obsolete. Use JSImportAttribute instead.")]
    public TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1)
        => InvokeUnmarshalled<T0, T1, object?, TResult>(identifier, arg0, arg1, null, 0);

    /// <inheritdoc />
    [Obsolete("This method is obsolete. Use JSImportAttribute instead.")]
    public TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2)
        => InvokeUnmarshalled<T0, T1, T2, TResult>(identifier, arg0, arg1, arg2, 0);
}
