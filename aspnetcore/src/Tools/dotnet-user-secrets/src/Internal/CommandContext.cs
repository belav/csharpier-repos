using Microsoft.Extensions.Tools.Internal;

namespace Microsoft.Extensions.SecretManager.Tools.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class CommandContext
{
    public CommandContext(SecretsStore store, IReporter reporter, IConsole console)
    {
        SecretStore = store;
        Reporter = reporter;
        Console = console;
    }

    public IConsole Console { get; }
    public IReporter Reporter { get; }
    public SecretsStore SecretStore { get; }
}
