using System.ComponentModel.DataAnnotations;

namespace HtmlGenerationWebSite.Models;

public class AClass
{
    public DayOfWeek DayOfWeek { get; set; }

    [DisplayFormat(DataFormatString = "Month: {0}")]
    public Month Month { get; set; }
}
