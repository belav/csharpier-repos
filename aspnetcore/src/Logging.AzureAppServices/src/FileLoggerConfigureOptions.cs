using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging.AzureAppServices;

internal sealed class FileLoggerConfigureOptions
    : BatchLoggerConfigureOptions,
        IConfigureOptions<AzureFileLoggerOptions>
{
    private readonly IWebAppContext _context;

    public FileLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
        : base(configuration, "AzureDriveEnabled")
    {
        _context = context;
    }

    public void Configure(AzureFileLoggerOptions options)
    {
        base.Configure(options);
        options.LogDirectory = Path.Combine(_context.HomeFolder, "LogFiles", "Application");
    }
}
