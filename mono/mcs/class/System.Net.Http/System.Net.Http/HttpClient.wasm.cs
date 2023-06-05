namespace System.Net.Http
{
    partial public class HttpClient
    {
        static HttpMessageHandler CreateDefaultHandler() =>
            new System.Net.Http.WebAssemblyHttpHandler();
    }
}
