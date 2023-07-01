using System.ComponentModel.DataAnnotations;

namespace FormatterWebSite;

// A System.Security.Principal.SecurityIdentifier like type that works on xplat
public class RecursiveIdentifier : IValidatableObject
{
    public RecursiveIdentifier(string identifier)
    {
        Value = identifier;
    }

    [Required]
    public string Value { get; }

    public RecursiveIdentifier AccountIdentifier => new RecursiveIdentifier(Value);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return Enumerable.Empty<ValidationResult>();
    }
}
