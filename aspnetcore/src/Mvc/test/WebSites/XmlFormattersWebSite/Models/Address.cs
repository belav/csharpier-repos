using System.ComponentModel.DataAnnotations;

namespace XmlFormattersWebSite;

public class Address
{
    [Required]
    public string State { get; set; }

    [Required]
    public int Zipcode { get; set; }
}
