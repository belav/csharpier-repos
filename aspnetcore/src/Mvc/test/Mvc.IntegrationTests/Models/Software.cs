using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests;

public class Software : Product
{
    public string Version { get; set; }

    [Required]
    public DateTime DatePurchased { get; set; }

    [Range(100, 200)]
    public override int Price { get; set; }

    [StringLength(10)]
    public new string Contact { get; set; }
}
