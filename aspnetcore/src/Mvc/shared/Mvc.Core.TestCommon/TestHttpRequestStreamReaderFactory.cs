using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace Microsoft.AspNetCore.Mvc;

public class TestHttpRequestStreamReaderFactory : IHttpRequestStreamReaderFactory
{
    public TextReader CreateReader(Stream stream, Encoding encoding)
    {
        return new HttpRequestStreamReader(stream, encoding);
    }
}
