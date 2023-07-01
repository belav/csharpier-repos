using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices.Test;

internal class OptionsWrapperMonitor<T> : IOptionsMonitor<T>
{
    public OptionsWrapperMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        return null;
    }

    public T Get(string name) => CurrentValue;

    public T CurrentValue { get; }
}
