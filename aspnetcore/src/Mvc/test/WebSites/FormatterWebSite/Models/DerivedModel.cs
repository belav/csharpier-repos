using System.ComponentModel.DataAnnotations;

namespace FormatterWebSite.Models;

public class DerivedModel : BaseModel, IModel
{
    [Required]
    [StringLength(10)]
    public string DerivedProperty { get; set; }
}
