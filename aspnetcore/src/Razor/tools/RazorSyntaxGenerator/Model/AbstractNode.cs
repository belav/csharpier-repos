using System.Collections.Generic;
using System.Xml.Serialization;

namespace RazorSyntaxGenerator;

public class AbstractNode : TreeType
{
    [XmlElement(ElementName = "Field", Type = typeof(Field))]
    public List<Field> Fields;
}
