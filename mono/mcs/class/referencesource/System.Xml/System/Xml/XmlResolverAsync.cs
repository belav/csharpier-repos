using System.Threading.Tasks;

namespace System.Xml
{
    partial public abstract class XmlResolver
    {
        public virtual Task<Object> GetEntityAsync(
            Uri absoluteUri,
            string role,
            Type ofObjectToReturn
        )
        {
            throw new NotImplementedException();
        }
    }
}
