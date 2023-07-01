using System.Xml;
using System.Xml.Serialization;

namespace RazorSyntaxGenerator;

public class Comment
{
    [XmlAnyElement]
    public XmlElement[] Body;
}
