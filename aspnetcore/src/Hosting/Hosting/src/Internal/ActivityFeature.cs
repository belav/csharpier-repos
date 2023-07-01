using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.Hosting;

/// <summary>
/// Default implementation for <see cref="IHttpActivityFeature"/>.
/// </summary>
internal sealed class ActivityFeature : IHttpActivityFeature
{
    internal ActivityFeature(Activity activity)
    {
        Activity = activity;
    }

    /// <inheritdoc />
    public Activity Activity { get; set; }
}
