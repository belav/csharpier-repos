using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.ConcurrencyLimiter;

internal sealed class StackPolicy : BasePolicy
{
    public StackPolicy(IOptions<QueuePolicyOptions> options)
        : base(options, QueueProcessingOrder.NewestFirst) { }
}
