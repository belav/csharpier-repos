using System.Runtime.Serialization;

namespace FormatterWebSite;

[DataContract]
public class Person
{
    public Person(string name)
    {
        Name = name;
    }

    [DataMember]
    public string Name { get; set; }
}
