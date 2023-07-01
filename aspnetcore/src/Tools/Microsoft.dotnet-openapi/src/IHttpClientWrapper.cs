using System;
using System.Threading.Tasks;
using Microsoft.DotNet.OpenApi;

namespace Microsoft.DotNet.Openapi.Tools;

internal interface IHttpClientWrapper : IDisposable
{
    Task<IHttpResponseMessageWrapper> GetResponseAsync(string url);
}
