// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Wasm.Performance.Driver;

internal sealed class Selenium
{
    const string SeleniumHost = "127.0.0.1";
    const int SeleniumPort = 4444;
    const bool RunHeadlessBrowser = true;

    const bool PoolForBrowserLogs = true;

    private static async ValueTask<Uri> WaitForServerAsync(
        string host,
        int port,
        CancellationToken cancellationToken
    )
    {
        var uri = new UriBuilder("http", host, port, "/wd/hub/").Uri;
        var httpClient = new HttpClient { BaseAddress = uri, Timeout = TimeSpan.FromSeconds(1) };

        Console.WriteLine($"Attempting to connect to Selenium Server running at {uri}");

        const int MaxRetries = 30;
        var retries = 0;

        while (retries < MaxRetries)
        {
            retries++;
            try
            {
                var response = (
                    await httpClient.GetAsync("status", cancellationToken)
                ).EnsureSuccessStatusCode();
                Console.WriteLine("Connected to Selenium");
                return uri;
            }
            catch
            {
                if (retries == 1)
                {
                    Console.WriteLine(
                        "Could not connect to selenium-server. Has it been started as yet?"
                    );
                }
            }

            await Task.Delay(1000, cancellationToken);
        }

        throw new Exception($"Unable to connect to selenium-server at {uri}");
    }

    public static async Task<RemoteWebDriver> CreateBrowser(
        CancellationToken cancellationToken,
        bool captureBrowserMemory = false
    )
    {
        var uri = await WaitForServerAsync(SeleniumHost, SeleniumPort, cancellationToken);

        var options = new ChromeOptions();

        if (RunHeadlessBrowser)
        {
            options.AddArgument("--headless");
        }

        if (captureBrowserMemory)
        {
            options.AddArgument("--enable-precise-memory-info");
        }

        // Chrome fails to load site resources if it fills up the /dev/shm partition,
        // so we add this argument to force using a temporary directory for shared memory files.
        options.AddArgument("--disable-dev-shm-usage");

        options.SetLoggingPreference(LogType.Browser, OpenQA.Selenium.LogLevel.All);

        var attempt = 0;
        const int MaxAttempts = 3;
        do
        {
            try
            {
                // The driver opens the browser window and tries to connect to it on the constructor.
                // Under heavy load, this can cause issues
                // To prevent this we let the client attempt several times to connect to the server, increasing
                // the max allowed timeout for a command on each attempt linearly.
                var driver = new CustomRemoteWebDriver(
                    uri,
                    options.ToCapabilities(),
                    TimeSpan.FromSeconds(60).Add(TimeSpan.FromSeconds(attempt * 60))
                );

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                if (PoolForBrowserLogs)
                {
                    // Run in background.
                    var logs = driver.Manage().Logs;
                    _ = Task.Run(async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));

                            var consoleLogs = logs.GetLog(LogType.Browser);
                            foreach (var entry in consoleLogs)
                            {
                                Console.WriteLine(
                                    $"[Browser Log]: {entry.Timestamp}: {entry.Message}"
                                );
                            }
                        }
                    });
                }

                return driver;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing RemoteWebDriver: {ex.Message}");
            }

            attempt++;
        } while (attempt < MaxAttempts);

        throw new InvalidOperationException(
            "Couldn't create a Selenium remote driver client. The server is irresponsive"
        );
    }

    // The WebDriver must implement ISupportsLogs to enable reading browser console logs.
    private sealed class CustomRemoteWebDriver : RemoteWebDriver, ISupportsLogs
    {
        public CustomRemoteWebDriver(
            Uri remoteAddress,
            ICapabilities desiredCapabilities,
            TimeSpan commandTimeout
        )
            : base(remoteAddress, desiredCapabilities, commandTimeout) { }
    }
}
