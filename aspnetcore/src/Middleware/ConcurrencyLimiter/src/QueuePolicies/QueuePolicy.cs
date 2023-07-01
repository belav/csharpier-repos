using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.ConcurrencyLimiter;

internal sealed class QueuePolicy : BasePolicy
{
    public QueuePolicy(IOptions<QueuePolicyOptions> options)
        : base(options, QueueProcessingOrder.OldestFirst) { }
}
