using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.HttpLogging;

internal class OptionsWrapperMonitor<T> : IOptionsMonitor<T>
{
    private event Action<T, string> _listener;

    public OptionsWrapperMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        _listener = listener;
        return null;
    }

    public T Get(string name) => CurrentValue;

    public T CurrentValue { get; }

    internal void InvokeChanged()
    {
        _listener.Invoke(CurrentValue, null);
    }
}
