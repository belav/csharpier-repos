using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomEncryptorSample;

public class CustomXmlDecryptor : IXmlDecryptor
{
    private readonly ILogger _logger;

    public CustomXmlDecryptor(IServiceProvider services)
    {
        _logger = services.GetRequiredService<ILoggerFactory>().CreateLogger<CustomXmlDecryptor>();
    }

    public XElement Decrypt(XElement encryptedElement)
    {
        if (encryptedElement == null)
        {
            throw new ArgumentNullException(nameof(encryptedElement));
        }

        return new XElement(encryptedElement.Elements().Single());
    }
}
