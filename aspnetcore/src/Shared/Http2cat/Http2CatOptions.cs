using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http2Cat;

internal sealed class Http2CatOptions
{
    public string Url { get; set; }
    public Func<Http2Utilities, Task> Scenaro { get; set; }
}
