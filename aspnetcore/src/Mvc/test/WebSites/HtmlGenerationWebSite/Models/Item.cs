using System.ComponentModel.DataAnnotations;

namespace HtmlGenerationWebSite.Models;

public class Item
{
    [UIHint("Common")]
    [Display(Name = "ItemName")]
    public string Name { get; set; }

    [UIHint("Common")]
    [Display(Name = "ItemNo")]
    public int Id { get; set; }

    [Display(Name = "ItemDesc")]
    public string Description { get; set; }
}
