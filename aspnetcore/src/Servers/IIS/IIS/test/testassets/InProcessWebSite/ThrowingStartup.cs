using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TestSite;

public class ThrowingStartup
{
    public void Configure(IApplicationBuilder app)
    {
        throw new InvalidOperationException("From Configure");
    }
}
