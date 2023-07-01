using System.ComponentModel.DataAnnotations;

namespace BasicWebSite.Models;

public class Product
{
    [Range(10, 100)]
    public int SampleInt { get; set; }

    [MinLength(15)]
    public string SampleString { get; set; }
}
