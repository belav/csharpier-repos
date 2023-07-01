using System.ComponentModel.DataAnnotations;

namespace RazorWebSite.Models;

public enum ModelEnum
{
    [Display(Name = "FirstOptionDisplay")]
    FirstOption,
    SecondOptions
}

public class EnumModel
{
    [Display(Name = "ModelEnum")]
    public ModelEnum Id { get; set; }
}
