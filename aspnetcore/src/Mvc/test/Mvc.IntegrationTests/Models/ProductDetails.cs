using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests;

public class ProductDetails
{
    [Required]
    public string Detail1 { get; set; }

    [Required]
    public string Detail2 { get; set; }

    [Required]
    public string Detail3 { get; set; }
}
