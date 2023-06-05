using System;
using System.Reflection;

namespace System.Net.Http
{
    partial public class HttpClient
    {
        static HttpMessageHandler CreateDefaultHandler()
        {
            return ObjCRuntime.RuntimeOptions.GetHttpMessageHandler();
        }
    }
}
