using System.ComponentModel.DataAnnotations;

namespace BasicWebSite.Models;

[DisplayColumn("Name")]
public class User
{
    [Required]
    [MinLength(4)]
    public string Name { get; set; }
    public string Address { get; set; }
}
