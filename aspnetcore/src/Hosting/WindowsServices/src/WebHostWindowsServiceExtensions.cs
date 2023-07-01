using System.ServiceProcess;

namespace Microsoft.AspNetCore.Hosting.WindowsServices;

/// <summary>
///     Extensions to <see cref="IWebHost"/> for hosting inside a Windows service.
/// </summary>
public static class WebHostWindowsServiceExtensions
{
    /// <summary>
    ///     Runs the specified web application inside a Windows service and blocks until the service is stopped.
    /// </summary>
    /// <param name="host">An instance of the <see cref="IWebHost"/> to host in the Windows service.</param>
    /// <example>
    ///     This example shows how to use <see cref="RunAsService"/>.
    ///     <code>
    ///         public class Program
    ///         {
    ///             public static void Main(string[] args)
    ///             {
    ///                 var config = WebHostConfiguration.GetDefault(args);
    ///
    ///                 var host = new WebHostBuilder()
    ///                     .UseConfiguration(config)
    ///                     .Build();
    ///
    ///                 // This call will block until the service is stopped.
    ///                 host.RunAsService();
    ///             }
    ///         }
    ///     </code>
    /// </example>
    public static void RunAsService(this IWebHost host)
    {
        var webHostService = new WebHostService(host);
        ServiceBase.Run(webHostService);
    }
}
