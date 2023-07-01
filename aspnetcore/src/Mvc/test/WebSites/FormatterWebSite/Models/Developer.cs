using System.ComponentModel.DataAnnotations;

namespace FormatterWebSite;

public class Developer
{
    [Required]
    public string Name { get; set; }

    public string Alias { get; set; }
}
