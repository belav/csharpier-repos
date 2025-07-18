// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Disables the request body size limit.
/// </summary>
/// <remarks>
/// Disabling the request body size limit can be a security concern in regards to uncontrolled
/// resource consumption, particularly if the request body is being buffered.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true
)]
public class DisableRequestSizeLimitAttribute
    : Attribute,
        IFilterFactory,
        IOrderedFilter,
        IRequestSizeLimitMetadata
{
    /// <summary>
    /// Gets the order value for determining the order of execution of filters. Filters execute in
    /// ascending numeric value of the <see cref="Order"/> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Filters are executed in an ordering determined by an ascending sort of the <see cref="Order"/> property.
    /// </para>
    /// <para>
    /// The default Order for this attribute is 900 because it must run before ValidateAntiForgeryTokenAttribute and
    /// after any filter which does authentication or login in order to allow them to behave as expected (ie Unauthenticated or Redirect instead of 400).
    /// </para>
    /// <para>
    /// Look at <see cref="IOrderedFilter.Order"/> for more detailed info.
    /// </para>
    /// </remarks>
    public int Order { get; set; } = 900;

    /// <inheritdoc />
    public bool IsReusable => true;

    /// <inheritdoc />
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<DisableRequestSizeLimitFilter>();
        return filter;
    }

    /// <inheritdoc />
    long? IRequestSizeLimitMetadata.MaxRequestBodySize => null;
}
