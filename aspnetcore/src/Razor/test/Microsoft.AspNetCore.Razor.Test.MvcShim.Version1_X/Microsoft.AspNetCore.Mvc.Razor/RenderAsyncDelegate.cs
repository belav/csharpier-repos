using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Razor;

public delegate Task RenderAsyncDelegate(TextWriter writer);
