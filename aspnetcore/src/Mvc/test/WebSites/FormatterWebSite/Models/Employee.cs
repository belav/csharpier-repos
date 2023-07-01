using System.ComponentModel.DataAnnotations;

namespace FormatterWebSite;

public class Employee
{
    [Range(10, 100)]
    public int Id { get; set; }

    [MinLength(15)]
    public string Name { get; set; }
}
