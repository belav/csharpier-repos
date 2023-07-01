using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Microsoft.AspNetCore.Mvc.DataAnnotations;

internal sealed class MinLengthAttributeAdapter : AttributeAdapterBase<MinLengthAttribute>
{
    private readonly string _min;

    public MinLengthAttributeAdapter(
        MinLengthAttribute attribute,
        IStringLocalizer? stringLocalizer
    )
        : base(attribute, stringLocalizer)
    {
        _min = Attribute.Length.ToString(CultureInfo.InvariantCulture);
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context));
        MergeAttribute(context.Attributes, "data-val-minlength-min", _min);
    }

    /// <inheritdoc />
    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        if (validationContext == null)
        {
            throw new ArgumentNullException(nameof(validationContext));
        }

        return GetErrorMessage(
            validationContext.ModelMetadata,
            validationContext.ModelMetadata.GetDisplayName(),
            Attribute.Length
        );
    }
}
