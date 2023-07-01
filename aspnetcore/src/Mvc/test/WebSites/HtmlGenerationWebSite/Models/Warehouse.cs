using System.ComponentModel.DataAnnotations;

namespace HtmlGenerationWebSite.Models;

public class Warehouse
{
    [MinLength(2)]
    public string City { get; set; }

    [Range(1, 100)]
    public int Product { get; set; }

    public Employee Employee { get; set; }
}
