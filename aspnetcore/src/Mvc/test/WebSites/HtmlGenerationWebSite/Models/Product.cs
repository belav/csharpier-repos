using System.ComponentModel.DataAnnotations;

namespace HtmlGenerationWebSite.Models;

public class Product
{
    [Required]
    public string ProductName { get; set; }

    public int Number { get; set; }

    public string Description { get; set; }

    public Uri HomePage { get; set; }
}
