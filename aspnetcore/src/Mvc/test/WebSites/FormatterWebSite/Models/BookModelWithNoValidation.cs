using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FormatterWebSite.Models;

public class BookModelWithNoValidation
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    [JsonRequired]
    [DataMember(IsRequired = true)]
    public string ISBN { get; set; }
}
