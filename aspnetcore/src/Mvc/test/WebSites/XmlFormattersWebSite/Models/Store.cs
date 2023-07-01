using System.ComponentModel.DataAnnotations;

namespace XmlFormattersWebSite;

public class Store
{
    [Required]
    public int Id { get; set; }

    [Required]
    public Address Address { get; set; }
}
