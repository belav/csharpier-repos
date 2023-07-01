using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ApiExplorerWebSite;

public class Product
{
    [BindRequired]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}
