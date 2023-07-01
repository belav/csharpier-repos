using System.Collections.Generic;
using System.Xml.Serialization;

namespace RazorSyntaxGenerator;

public class Field
{
    [XmlAttribute]
    public string Name;

    [XmlAttribute]
    public string Type;

    [XmlAttribute]
    public string Optional;

    [XmlAttribute]
    public string Override;

    [XmlAttribute]
    public string New;

    [XmlElement(ElementName = "Kind", Type = typeof(Kind))]
    public List<Kind> Kinds;

    [XmlElement]
    public Comment PropertyComment;
}
