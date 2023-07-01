using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests;

[ModelMetadataType(typeof(Software))]
public class SoftwareViewModel
{
    [RegularExpression("^[0-9]*$")]
    public string Version { get; set; }

    public DateTime DatePurchased { get; set; }

    public int Price { get; set; }

    public string Contact { get; set; }

    public string Country { get; set; }

    public string Name { get; set; }

    public string Category { get; set; }

    public string CompanyName { get; set; }
}
