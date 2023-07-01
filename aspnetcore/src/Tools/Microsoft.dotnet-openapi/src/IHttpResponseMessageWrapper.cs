using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.DotNet.OpenApi;

public interface IHttpResponseMessageWrapper : IDisposable
{
    Task<Stream> Stream { get; }
    ContentDispositionHeaderValue ContentDisposition();
    HttpStatusCode StatusCode { get; }
    bool IsSuccessCode();
}
