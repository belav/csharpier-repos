using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FormatterWebSite;

[KnownType(typeof(DerivedDummyClass))]
[XmlInclude(typeof(DerivedDummyClass))]
public class DummyClass
{
    public int SampleInt { get; set; }
}
