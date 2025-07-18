// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures;

/// <summary>
/// Default implementation of <see cref="IHtmlHelper"/>.
/// </summary>
public class HtmlHelper : IHtmlHelper, IViewContextAware
{
    /// <summary>
    /// CSS class name for input validation.
    /// </summary>
    public static readonly string ValidationInputCssClassName = "input-validation-error";

    /// <summary>
    /// CSS class name for valid input validation.
    /// </summary>
    public static readonly string ValidationInputValidCssClassName = "input-validation-valid";

    /// <summary>
    /// CSS class name for field validation error.
    /// </summary>
    public static readonly string ValidationMessageCssClassName = "field-validation-error";

    /// <summary>
    /// CSS class name for valid field validation.
    /// </summary>
    public static readonly string ValidationMessageValidCssClassName = "field-validation-valid";

    /// <summary>
    /// CSS class name for validation summary errors.
    /// </summary>
    public static readonly string ValidationSummaryCssClassName = "validation-summary-errors";

    /// <summary>
    /// CSS class name for valid validation summary.
    /// </summary>
    public static readonly string ValidationSummaryValidCssClassName = "validation-summary-valid";

    private readonly IHtmlGenerator _htmlGenerator;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly HtmlEncoder _htmlEncoder;
    private readonly IViewBufferScope _bufferScope;

    private ViewContext _viewContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlHelper"/> class.
    /// </summary>
    public HtmlHelper(
        IHtmlGenerator htmlGenerator,
        ICompositeViewEngine viewEngine,
        IModelMetadataProvider metadataProvider,
        IViewBufferScope bufferScope,
        HtmlEncoder htmlEncoder,
        UrlEncoder urlEncoder
    )
    {
        ArgumentNullException.ThrowIfNull(htmlGenerator);
        ArgumentNullException.ThrowIfNull(viewEngine);
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentNullException.ThrowIfNull(bufferScope);
        ArgumentNullException.ThrowIfNull(htmlEncoder);
        ArgumentNullException.ThrowIfNull(urlEncoder);

        _viewEngine = viewEngine;
        _htmlGenerator = htmlGenerator;
        _htmlEncoder = htmlEncoder;
        _bufferScope = bufferScope;
        MetadataProvider = metadataProvider;
        UrlEncoder = urlEncoder;
    }

    /// <inheritdoc />
    public Html5DateRenderingMode Html5DateRenderingMode
    {
        get => ViewContext.Html5DateRenderingMode;
        set => ViewContext.Html5DateRenderingMode = value;
    }

    /// <inheritdoc />
    public string IdAttributeDotReplacement => _htmlGenerator.IdAttributeDotReplacement;

    /// <inheritdoc />
    public ViewContext ViewContext
    {
        get
        {
            if (_viewContext == null)
            {
                throw new InvalidOperationException(Resources.HtmlHelper_NotContextualized);
            }

            return _viewContext;
        }
        private set => _viewContext = value;
    }

    /// <inheritdoc />
    public dynamic ViewBag => ViewContext.ViewBag;

    /// <inheritdoc />
    public ViewDataDictionary ViewData => ViewContext.ViewData;

    /// <inheritdoc />
    public ITempDataDictionary TempData => ViewContext.TempData;

    /// <inheritdoc />
    public UrlEncoder UrlEncoder { get; }

    /// <inheritdoc />
    public IModelMetadataProvider MetadataProvider { get; }

    /// <summary>
    /// Creates a dictionary from an object, by adding each public instance property as a key with its associated
    /// value to the dictionary. It will expose public properties from derived types as well. This is typically
    /// used with objects of an anonymous type.
    ///
    /// If the <paramref name="value"/> is already an <see cref="IDictionary{String, Object}"/> instance, then it
    /// is returned as-is.
    /// <example>
    /// <c>new { data_name="value" }</c> will translate to the entry <c>{ "data_name", "value" }</c>
    /// in the resulting dictionary.
    /// </example>
    /// </summary>
    /// <param name="value">The <see cref="object"/> to be converted.</param>
    /// <returns>The created dictionary of property names and property values.</returns>
    public static IDictionary<string, object> ObjectToDictionary(object value)
    {
        return PropertyHelper.ObjectToDictionary(value);
    }

    /// <summary>
    /// Creates a dictionary of HTML attributes from the input object,
    /// translating underscores to dashes in each public instance property.
    /// </summary>
    /// <param name="htmlAttributes">Anonymous object describing HTML attributes.</param>
    /// <returns>A dictionary that represents HTML attributes.</returns>
    /// <remarks>
    /// If the object is already an <see cref="IDictionary{String, Object}"/> instance, then a shallow copy is
    /// returned.
    /// <example>
    /// <c>new { data_name="value" }</c> will translate to the entry <c>{ "data-name", "value" }</c>
    /// in the resulting dictionary.
    /// </example>
    /// </remarks>
    public static IDictionary<string, object> AnonymousObjectToHtmlAttributes(object htmlAttributes)
    {
        if (htmlAttributes is IDictionary<string, object> dictionary)
        {
            return new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        if (htmlAttributes != null)
        {
            foreach (
                var helper in HtmlAttributePropertyHelper.GetProperties(htmlAttributes.GetType())
            )
            {
                dictionary[helper.Name] = helper.GetValue(htmlAttributes);
            }
        }

        return dictionary;
    }

    /// <summary>
    /// Sets the <see cref="ViewContext"/>.
    /// </summary>
    /// <param name="viewContext">The context to use.</param>
    public virtual void Contextualize(ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        ViewContext = viewContext;
    }

    /// <inheritdoc />
    public IHtmlContent ActionLink(
        string linkText,
        string actionName,
        string controllerName,
        string protocol,
        string hostname,
        string fragment,
        object routeValues,
        object htmlAttributes
    )
    {
        ArgumentNullException.ThrowIfNull(linkText);

        var tagBuilder = _htmlGenerator.GenerateActionLink(
            ViewContext,
            linkText,
            actionName,
            controllerName,
            protocol,
            hostname,
            fragment,
            routeValues,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <inheritdoc />
    public IHtmlContent AntiForgeryToken()
    {
        var html = _htmlGenerator.GenerateAntiforgery(ViewContext);
        return html ?? HtmlString.Empty;
    }

    /// <inheritdoc />
    public MvcForm BeginForm(
        string actionName,
        string controllerName,
        object routeValues,
        FormMethod method,
        bool? antiforgery,
        object htmlAttributes
    )
    {
        // Push the new FormContext; MvcForm.GenerateEndForm() does the corresponding pop.
        _viewContext.FormContext = new FormContext { CanRenderAtEndOfForm = true };

        return GenerateForm(
            actionName,
            controllerName,
            routeValues,
            method,
            antiforgery,
            htmlAttributes
        );
    }

    /// <inheritdoc />
    public MvcForm BeginRouteForm(
        string routeName,
        object routeValues,
        FormMethod method,
        bool? antiforgery,
        object htmlAttributes
    )
    {
        // Push the new FormContext; MvcForm.GenerateEndForm() does the corresponding pop.
        _viewContext.FormContext = new FormContext { CanRenderAtEndOfForm = true };

        return GenerateRouteForm(routeName, routeValues, method, antiforgery, htmlAttributes);
    }

    /// <inheritdoc />
    public void EndForm()
    {
        var mvcForm = CreateForm();
        mvcForm.EndForm();
    }

    /// <inheritdoc />
    public IHtmlContent CheckBox(string expression, bool? isChecked, object htmlAttributes)
    {
        return GenerateCheckBox(
            modelExplorer: null,
            expression: expression,
            isChecked: isChecked,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public string Encode(string value)
    {
        return _htmlGenerator.Encode(value);
    }

    /// <inheritdoc />
    public string Encode(object value)
    {
        return _htmlGenerator.Encode(value);
    }

    /// <inheritdoc />
    public string FormatValue(object value, string format)
    {
        return _htmlGenerator.FormatValue(value, format);
    }

    /// <inheritdoc />
    public string GenerateIdFromName(string fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);

        return NameAndIdProvider.CreateSanitizedId(
            ViewContext,
            fullName,
            IdAttributeDotReplacement
        );
    }

    /// <inheritdoc />
    public IHtmlContent Display(
        string expression,
        string templateName,
        string htmlFieldName,
        object additionalViewData
    )
    {
        var metadata = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );

        return GenerateDisplay(
            metadata,
            htmlFieldName ?? GetExpressionText(expression),
            templateName,
            additionalViewData
        );
    }

    /// <inheritdoc />
    public string DisplayName(string expression)
    {
        var modelExplorer = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );
        return GenerateDisplayName(modelExplorer, expression);
    }

    /// <inheritdoc />
    public string DisplayText(string expression)
    {
        var modelExplorer = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );
        return GenerateDisplayText(modelExplorer);
    }

    /// <inheritdoc />
    public IHtmlContent DropDownList(
        string expression,
        IEnumerable<SelectListItem> selectList,
        string optionLabel,
        object htmlAttributes
    )
    {
        return GenerateDropDown(
            modelExplorer: null,
            expression: expression,
            selectList: selectList,
            optionLabel: optionLabel,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public IHtmlContent Editor(
        string expression,
        string templateName,
        string htmlFieldName,
        object additionalViewData
    )
    {
        var modelExplorer = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );

        return GenerateEditor(
            modelExplorer,
            htmlFieldName ?? GetExpressionText(expression),
            templateName,
            additionalViewData
        );
    }

    /// <inheritdoc />
    public IEnumerable<SelectListItem> GetEnumSelectList<TEnum>()
        where TEnum : struct
    {
        var type = typeof(TEnum);
        var metadata = MetadataProvider.GetMetadataForType(type);
        if (!metadata.IsEnum || metadata.IsFlagsEnum)
        {
            var message = Resources.FormatHtmlHelper_TypeNotSupported_ForGetEnumSelectList(
                type.FullName,
                nameof(Enum).ToLowerInvariant(),
                nameof(FlagsAttribute)
            );
            throw new ArgumentException(message, nameof(TEnum));
        }

        return GetEnumSelectList(metadata);
    }

    /// <inheritdoc />
    public IEnumerable<SelectListItem> GetEnumSelectList(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        var metadata = MetadataProvider.GetMetadataForType(enumType);
        if (!metadata.IsEnum || metadata.IsFlagsEnum)
        {
            var message = Resources.FormatHtmlHelper_TypeNotSupported_ForGetEnumSelectList(
                enumType.FullName,
                nameof(Enum).ToLowerInvariant(),
                nameof(FlagsAttribute)
            );
            throw new ArgumentException(message, nameof(enumType));
        }

        return GetEnumSelectList(metadata);
    }

    /// <inheritdoc />
    public IHtmlContent Hidden(string expression, object value, object htmlAttributes)
    {
        return GenerateHidden(
            modelExplorer: null,
            expression: expression,
            value: value,
            useViewData: (value == null),
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public string Id(string expression)
    {
        return GenerateId(expression);
    }

    /// <inheritdoc />
    public IHtmlContent Label(string expression, string labelText, object htmlAttributes)
    {
        var modelExplorer = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );
        return GenerateLabel(modelExplorer, expression, labelText, htmlAttributes);
    }

    /// <inheritdoc />
    public IHtmlContent ListBox(
        string expression,
        IEnumerable<SelectListItem> selectList,
        object htmlAttributes
    )
    {
        return GenerateListBox(
            modelExplorer: null,
            expression: expression,
            selectList: selectList,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public string Name(string expression)
    {
        return GenerateName(expression);
    }

    /// <inheritdoc />
    public async Task<IHtmlContent> PartialAsync(
        string partialViewName,
        object model,
        ViewDataDictionary viewData
    )
    {
        ArgumentNullException.ThrowIfNull(partialViewName);

        var viewBuffer = new ViewBuffer(
            _bufferScope,
            partialViewName,
            ViewBuffer.PartialViewPageSize
        );
        using (var writer = new ViewBufferTextWriter(viewBuffer, Encoding.UTF8))
        {
            await RenderPartialCoreAsync(partialViewName, model, viewData, writer);
            return viewBuffer;
        }
    }

    /// <inheritdoc />
    public Task RenderPartialAsync(
        string partialViewName,
        object model,
        ViewDataDictionary viewData
    )
    {
        ArgumentNullException.ThrowIfNull(partialViewName);

        return RenderPartialCoreAsync(partialViewName, model, viewData, ViewContext.Writer);
    }

    /// <summary>
    /// Generate a display.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="htmlFieldName">The name of the html field.</param>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="additionalViewData">The additional view data, either an <see cref="IDictionary{String, Object}"/> or some other object whose public properties will be merged with the <see cref="T:ViewDataDictionary" />.</param>
    /// <returns><see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateDisplay(
        ModelExplorer modelExplorer,
        string htmlFieldName,
        string templateName,
        object additionalViewData
    )
    {
        var templateBuilder = new TemplateBuilder(
            _viewEngine,
            _bufferScope,
            ViewContext,
            ViewData,
            modelExplorer,
            htmlFieldName,
            templateName,
            readOnly: true,
            additionalViewData: additionalViewData
        );

        return templateBuilder.Build();
    }

    /// <summary>
    /// Render a partial view.
    /// </summary>
    /// <param name="partialViewName">The name of the partial view.</param>
    /// <param name="model">The model.</param>
    /// <param name="viewData">The view data.</param>
    /// <param name="writer">The <see cref="TextWriter"/>.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    protected virtual async Task RenderPartialCoreAsync(
        string partialViewName,
        object model,
        ViewDataDictionary viewData,
        TextWriter writer
    )
    {
        ArgumentNullException.ThrowIfNull(partialViewName);

        var viewEngineResult = _viewEngine.GetView(
            ViewContext.ExecutingFilePath,
            partialViewName,
            isMainPage: false
        );
        var originalLocations = viewEngineResult.SearchedLocations;
        if (!viewEngineResult.Success)
        {
            viewEngineResult = _viewEngine.FindView(
                ViewContext,
                partialViewName,
                isMainPage: false
            );
        }

        if (!viewEngineResult.Success)
        {
            var locations = string.Empty;
            if (originalLocations.Any())
            {
                locations =
                    Environment.NewLine + string.Join(Environment.NewLine, originalLocations);
            }

            if (viewEngineResult.SearchedLocations.Any())
            {
                locations +=
                    Environment.NewLine
                    + string.Join(Environment.NewLine, viewEngineResult.SearchedLocations);
            }

            throw new InvalidOperationException(
                Resources.FormatViewEngine_PartialViewNotFound(partialViewName, locations)
            );
        }

        var view = viewEngineResult.View;
        using (view as IDisposable)
        {
            // Determine which ViewData we should use to construct a new ViewData
            var baseViewData = viewData ?? ViewData;

            var newViewData = new ViewDataDictionary<object>(baseViewData, model);
            var viewContext = new ViewContext(ViewContext, view, newViewData, writer);

            await viewEngineResult.View.RenderAsync(viewContext);
        }
    }

    /// <inheritdoc />
    public IHtmlContent Password(string expression, object value, object htmlAttributes)
    {
        return GeneratePassword(
            modelExplorer: null,
            expression: expression,
            value: value,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public IHtmlContent RadioButton(
        string expression,
        object value,
        bool? isChecked,
        object htmlAttributes
    )
    {
        return GenerateRadioButton(
            modelExplorer: null,
            expression: expression,
            value: value,
            isChecked: isChecked,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public IHtmlContent Raw(string value)
    {
        return new HtmlString(value);
    }

    /// <inheritdoc />
    public IHtmlContent Raw(object value)
    {
        return new HtmlString(value?.ToString());
    }

    /// <inheritdoc />
    public IHtmlContent RouteLink(
        string linkText,
        string routeName,
        string protocol,
        string hostName,
        string fragment,
        object routeValues,
        object htmlAttributes
    )
    {
        ArgumentNullException.ThrowIfNull(linkText);

        var tagBuilder = _htmlGenerator.GenerateRouteLink(
            ViewContext,
            linkText,
            routeName,
            protocol,
            hostName,
            fragment,
            routeValues,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <inheritdoc />
    public IHtmlContent ValidationMessage(
        string expression,
        string message,
        object htmlAttributes,
        string tag
    )
    {
        return GenerateValidationMessage(
            modelExplorer: null,
            expression: expression,
            message: message,
            tag: tag,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public IHtmlContent ValidationSummary(
        bool excludePropertyErrors,
        string message,
        object htmlAttributes,
        string tag
    )
    {
        return GenerateValidationSummary(excludePropertyErrors, message, htmlAttributes, tag);
    }

    /// <summary>
    /// Returns the HTTP method that handles form input (GET or POST) as a string.
    /// </summary>
    /// <param name="method">The HTTP method that handles the form.</param>
    /// <returns>The form method string, either "get" or "post".</returns>
    public static string GetFormMethodString(FormMethod method)
    {
        switch (method)
        {
            case FormMethod.Get:
                return "get";
            case FormMethod.Post:
                return "post";
            default:
                return "post";
        }
    }

    /// <inheritdoc />
    public IHtmlContent TextArea(
        string expression,
        string value,
        int rows,
        int columns,
        object htmlAttributes
    )
    {
        var modelExplorer = ExpressionMetadataProvider.FromStringExpression(
            expression,
            ViewData,
            MetadataProvider
        );
        if (value != null)
        {
            // As a special case we allow treating a string value as a model of arbitrary type.
            // So pass through the string representation, even though the ModelMetadata might
            // be for some other type.
            //
            // We do this because thought we're displaying something as a string, we want to have
            // the right set of validation attributes.
            modelExplorer = new ModelExplorer(
                MetadataProvider,
                modelExplorer.Container,
                modelExplorer.Metadata,
                value
            );
        }

        return GenerateTextArea(modelExplorer, expression, rows, columns, htmlAttributes);
    }

    /// <inheritdoc />
    public IHtmlContent TextBox(
        string expression,
        object value,
        string format,
        object htmlAttributes
    )
    {
        return GenerateTextBox(
            modelExplorer: null,
            expression: expression,
            value: value,
            format: format,
            htmlAttributes: htmlAttributes
        );
    }

    /// <inheritdoc />
    public string Value(string expression, string format)
    {
        return GenerateValue(expression, value: null, format: format, useViewData: true);
    }

    /// <summary>
    /// Override this method to return an <see cref="MvcForm"/> subclass. That subclass may change
    /// <see cref="EndForm()"/> behavior.
    /// </summary>
    /// <returns>A new <see cref="MvcForm"/> instance.</returns>
    protected virtual MvcForm CreateForm()
    {
        return new MvcForm(ViewContext, _htmlEncoder);
    }

    /// <summary>
    /// Generate a check box.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="isChecked">Whether the box should be checked.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns></returns>
    protected virtual IHtmlContent GenerateCheckBox(
        ModelExplorer modelExplorer,
        string expression,
        bool? isChecked,
        object htmlAttributes
    )
    {
        var checkbox = _htmlGenerator.GenerateCheckBox(
            ViewContext,
            modelExplorer,
            expression,
            isChecked,
            htmlAttributes
        );

        if (checkbox == null)
        {
            return HtmlString.Empty;
        }

        if (ViewContext.CheckBoxHiddenInputRenderMode == CheckBoxHiddenInputRenderMode.None)
        {
            return checkbox;
        }

        var hiddenForCheckbox = _htmlGenerator.GenerateHiddenForCheckbox(
            ViewContext,
            modelExplorer,
            expression
        );
        if (hiddenForCheckbox == null)
        {
            return HtmlString.Empty;
        }

        if (
            !hiddenForCheckbox.Attributes.ContainsKey("name")
            && checkbox.Attributes.TryGetValue("name", out var name)
        )
        {
            // The checkbox and hidden elements should have the same name attribute value. Attributes will match
            // if both are present because both have a generated value. Reach here in the special case where user
            // provided a non-empty fallback name.
            hiddenForCheckbox.MergeAttribute("name", name);
        }

        if (
            ViewContext.CheckBoxHiddenInputRenderMode == CheckBoxHiddenInputRenderMode.EndOfForm
            && ViewContext.FormContext.CanRenderAtEndOfForm
        )
        {
            ViewContext.FormContext.EndOfFormContent.Add(hiddenForCheckbox);
            return checkbox;
        }

        return new HtmlContentBuilder(capacity: 2)
            .AppendHtml(checkbox)
            .AppendHtml(hiddenForCheckbox);
    }

    /// <summary>
    /// Generate display name.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <returns>The display name.</returns>
    protected virtual string GenerateDisplayName(ModelExplorer modelExplorer, string expression)
    {
        ArgumentNullException.ThrowIfNull(modelExplorer);

        // We don't call ModelMetadata.GetDisplayName here because
        // we want to fall back to the field name rather than the ModelType.
        // This is similar to how the GenerateLabel get the text of a label.
        var resolvedDisplayName =
            modelExplorer.Metadata.DisplayName ?? modelExplorer.Metadata.PropertyName;
        if (resolvedDisplayName == null && expression != null)
        {
            var index = expression.LastIndexOf('.');
            if (index == -1)
            {
                // Expression does not contain a dot separator.
                resolvedDisplayName = expression;
            }
            else
            {
                resolvedDisplayName = expression.Substring(index + 1);
            }
        }

        return resolvedDisplayName ?? string.Empty;
    }

    /// <summary>
    /// Generate display text.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <returns>The text.</returns>
    protected virtual string GenerateDisplayText(ModelExplorer modelExplorer)
    {
        return modelExplorer.GetSimpleDisplayText() ?? string.Empty;
    }

    /// <summary>
    /// Generate a drop down.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="selectList">The select list.</param>
    /// <param name="optionLabel">The option label.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected IHtmlContent GenerateDropDown(
        ModelExplorer modelExplorer,
        string expression,
        IEnumerable<SelectListItem> selectList,
        string optionLabel,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateSelect(
            ViewContext,
            modelExplorer,
            optionLabel,
            expression,
            selectList,
            allowMultiple: false,
            htmlAttributes: htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate editor.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="htmlFieldName">The name of the html field.</param>
    /// <param name="templateName">The name of the template</param>
    /// <param name="additionalViewData">Additional view data.</param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateEditor(
        ModelExplorer modelExplorer,
        string htmlFieldName,
        string templateName,
        object additionalViewData
    )
    {
        var templateBuilder = new TemplateBuilder(
            _viewEngine,
            _bufferScope,
            ViewContext,
            ViewData,
            modelExplorer,
            htmlFieldName,
            templateName,
            readOnly: false,
            additionalViewData: additionalViewData
        );

        return templateBuilder.Build();
    }

    /// <summary>
    /// Renders a &lt;form&gt; start tag to the response. When the user submits the form, the action with name
    /// <paramref name="actionName"/> will process the request.
    /// </summary>
    /// <param name="actionName">The name of the action method.</param>
    /// <param name="controllerName">The name of the controller.</param>
    /// <param name="routeValues">
    /// An <see cref="object"/> that contains the parameters for a route. The parameters are retrieved through
    /// reflection by examining the properties of the <see cref="object"/>. This <see cref="object"/> is typically
    /// created using <see cref="object"/> initializer syntax. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the route parameters.
    /// </param>
    /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
    /// <param name="antiforgery">
    /// If <c>true</c>, &lt;form&gt; elements will include an antiforgery token.
    /// If <c>false</c>, suppresses the generation an &lt;input&gt; of type "hidden" with an antiforgery token.
    /// If <c>null</c>, &lt;form&gt; elements will include an antiforgery token only if
    /// <paramref name="method"/> is not <see cref="FormMethod.Get"/>.
    /// </param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>
    /// An <see cref="MvcForm"/> instance which renders the &lt;/form&gt; end tag when disposed.
    /// </returns>
    /// <remarks>
    /// In this context, "renders" means the method writes its output using <see cref="ViewContext.Writer"/>.
    /// </remarks>
    protected virtual MvcForm GenerateForm(
        string actionName,
        string controllerName,
        object routeValues,
        FormMethod method,
        bool? antiforgery,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateForm(
            ViewContext,
            actionName,
            controllerName,
            routeValues,
            GetFormMethodString(method),
            htmlAttributes
        );
        if (tagBuilder != null)
        {
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            tagBuilder.WriteTo(ViewContext.Writer, _htmlEncoder);
        }

        var shouldGenerateAntiforgery = antiforgery ?? method != FormMethod.Get;
        if (shouldGenerateAntiforgery)
        {
            ViewContext.FormContext.EndOfFormContent.Add(
                _htmlGenerator.GenerateAntiforgery(ViewContext)
            );
        }

        return CreateForm();
    }

    /// <summary>
    /// Renders a &lt;form&gt; start tag to the response. The route with name <paramref name="routeName"/>
    /// generates the &lt;form&gt;'s <c>action</c> attribute value.
    /// </summary>
    /// <param name="routeName">The name of the route.</param>
    /// <param name="routeValues">
    /// An <see cref="object"/> that contains the parameters for a route. The parameters are retrieved through
    /// reflection by examining the properties of the <see cref="object"/>. This <see cref="object"/> is typically
    /// created using <see cref="object"/> initializer syntax. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the route parameters.
    /// </param>
    /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
    /// <param name="antiforgery">
    /// If <c>true</c>, &lt;form&gt; elements will include an antiforgery token.
    /// If <c>false</c>, suppresses the generation an &lt;input&gt; of type "hidden" with an antiforgery token.
    /// If <c>null</c>, &lt;form&gt; elements will include an antiforgery token only if
    /// <paramref name="method"/> is not <see cref="FormMethod.Get"/>.
    /// </param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>
    /// An <see cref="MvcForm"/> instance which renders the &lt;/form&gt; end tag when disposed.
    /// </returns>
    /// <remarks>
    /// In this context, "renders" means the method writes its output using <see cref="ViewContext.Writer"/>.
    /// </remarks>
    protected virtual MvcForm GenerateRouteForm(
        string routeName,
        object routeValues,
        FormMethod method,
        bool? antiforgery,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateRouteForm(
            ViewContext,
            routeName,
            routeValues,
            GetFormMethodString(method),
            htmlAttributes
        );
        if (tagBuilder != null)
        {
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            tagBuilder.WriteTo(ViewContext.Writer, _htmlEncoder);
        }

        var shouldGenerateAntiforgery = antiforgery ?? method != FormMethod.Get;
        if (shouldGenerateAntiforgery)
        {
            ViewContext.FormContext.EndOfFormContent.Add(
                _htmlGenerator.GenerateAntiforgery(ViewContext)
            );
        }

        return CreateForm();
    }

    /// <summary>
    /// Generate a hidden.
    /// </summary>
    /// <param name="modelExplorer"></param>
    /// <param name="expression"></param>
    /// <param name="value"></param>
    /// <param name="useViewData"></param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateHidden(
        ModelExplorer modelExplorer,
        string expression,
        object value,
        bool useViewData,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateHidden(
            ViewContext,
            modelExplorer,
            expression,
            value,
            useViewData,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate an id.
    /// </summary>
    /// <param name="expression">The expresion.</param>
    /// <returns>The id.</returns>
    protected virtual string GenerateId(string expression)
    {
        var fullName = NameAndIdProvider.GetFullHtmlFieldName(ViewContext, expression);

        return GenerateIdFromName(fullName);
    }

    /// <summary>
    /// Generate a label.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expresion.</param>
    /// <param name="labelText">The label text.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateLabel(
        ModelExplorer modelExplorer,
        string expression,
        string labelText,
        object htmlAttributes
    )
    {
        ArgumentNullException.ThrowIfNull(modelExplorer);

        var tagBuilder = _htmlGenerator.GenerateLabel(
            ViewContext,
            modelExplorer,
            expression,
            labelText,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        // Do not generate an empty <label> element unless user passed string.Empty for the label text. This is
        // primarily done for back-compatibility. (Note HtmlContentBuilder ignores (no-ops) an attempt to add
        // string.Empty. So tagBuilder.HasInnerHtml isn't sufficient here.)
        if (!tagBuilder.HasInnerHtml && labelText == null)
        {
            if (tagBuilder.Attributes.Count == 0)
            {
                // Element has no content and no attributes.
                return HtmlString.Empty;
            }
            else if (
                tagBuilder.Attributes.Count == 1
                && tagBuilder.Attributes.TryGetValue("for", out var forAttribute)
                && string.IsNullOrEmpty(forAttribute)
            )
            {
                // Element has no content and only an empty (therefore useless) "for" attribute.
                return HtmlString.Empty;
            }
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a list box.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="selectList">An enumeration of <see cref="SelectListItem"/>.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected IHtmlContent GenerateListBox(
        ModelExplorer modelExplorer,
        string expression,
        IEnumerable<SelectListItem> selectList,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateSelect(
            ViewContext,
            modelExplorer,
            optionLabel: null,
            expression: expression,
            selectList: selectList,
            allowMultiple: true,
            htmlAttributes: htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Geneate a name.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The name.</returns>
    protected virtual string GenerateName(string expression)
    {
        var fullName = NameAndIdProvider.GetFullHtmlFieldName(ViewContext, expression);
        return fullName;
    }

    /// <summary>
    /// Generate a password.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="value">The password value.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GeneratePassword(
        ModelExplorer modelExplorer,
        string expression,
        object value,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GeneratePassword(
            ViewContext,
            modelExplorer,
            expression,
            value,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a radio button.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="value">The value.</param>
    /// <param name="isChecked">If the radio button is checked.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateRadioButton(
        ModelExplorer modelExplorer,
        string expression,
        object value,
        bool? isChecked,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateRadioButton(
            ViewContext,
            modelExplorer,
            expression,
            value,
            isChecked,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a text area.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateTextArea(
        ModelExplorer modelExplorer,
        string expression,
        int rows,
        int columns,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateTextArea(
            ViewContext,
            modelExplorer,
            expression,
            rows,
            columns,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generates a text box.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="value">The value.</param>
    /// <param name="format">The format.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateTextBox(
        ModelExplorer modelExplorer,
        string expression,
        object value,
        string format,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateTextBox(
            ViewContext,
            modelExplorer,
            expression,
            value,
            format,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a validation message.
    /// </summary>
    /// <param name="modelExplorer">The <see cref="ModelExplorer"/>.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="message">The validation message.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateValidationMessage(
        ModelExplorer modelExplorer,
        string expression,
        string message,
        string tag,
        object htmlAttributes
    )
    {
        var tagBuilder = _htmlGenerator.GenerateValidationMessage(
            ViewContext,
            modelExplorer,
            expression,
            message,
            tag,
            htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a validation summary.
    /// </summary>
    /// <param name="excludePropertyErrors">Whether to exclude property errors.</param>
    /// <param name="message">The validation message.</param>
    /// <param name="htmlAttributes">
    /// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
    /// <see cref="IDictionary{String, Object}"/> instance containing the HTML attributes.
    /// </param>
    /// <param name="tag">The tag.</param>
    /// <returns>The <see cref="IHtmlContent"/>.</returns>
    protected virtual IHtmlContent GenerateValidationSummary(
        bool excludePropertyErrors,
        string message,
        object htmlAttributes,
        string tag
    )
    {
        var tagBuilder = _htmlGenerator.GenerateValidationSummary(
            ViewContext,
            excludePropertyErrors,
            message,
            headerTag: tag,
            htmlAttributes: htmlAttributes
        );
        if (tagBuilder == null)
        {
            return HtmlString.Empty;
        }

        return tagBuilder;
    }

    /// <summary>
    /// Generate a value.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="value">The value.</param>
    /// <param name="format">The format.</param>
    /// <param name="useViewData">Whether to use view data.</param>
    /// <returns>The value.</returns>
    protected virtual string GenerateValue(
        string expression,
        object value,
        string format,
        bool useViewData
    )
    {
        var fullName = NameAndIdProvider.GetFullHtmlFieldName(ViewContext, expression);
        var attemptedValue = (string)
            DefaultHtmlGenerator.GetModelStateValue(ViewContext, fullName, typeof(string));

        string resolvedValue;
        if (attemptedValue != null)
        {
            // case 1: if ModelState has a value then it's already formatted so ignore format string
            resolvedValue = attemptedValue;
        }
        else if (useViewData)
        {
            // case 2: format the value from ViewData
            resolvedValue = DefaultHtmlGenerator.EvalString(ViewContext, expression, format);
        }
        else
        {
            // case 3: format the explicit value from ModelMetadata
            resolvedValue = FormatValue(value, format);
        }

        return resolvedValue;
    }

    /// <summary>
    /// Returns a select list for the given <paramref name="metadata"/>.
    /// </summary>
    /// <param name="metadata"><see cref="ModelMetadata"/> to generate a select list for.</param>
    /// <returns>
    /// An <see cref="IEnumerable{SelectListItem}"/> containing the select list for the given
    /// <paramref name="metadata"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="metadata"/>'s <see cref="ModelMetadata.ModelType"/> is not an <see cref="Enum"/>
    /// or if it has a <see cref="FlagsAttribute"/>.
    /// </exception>
    protected virtual IEnumerable<SelectListItem> GetEnumSelectList(ModelMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (!metadata.IsEnum || metadata.IsFlagsEnum)
        {
            var message = Resources.FormatHtmlHelper_TypeNotSupported_ForGetEnumSelectList(
                metadata.ModelType.FullName,
                nameof(Enum).ToLowerInvariant(),
                nameof(FlagsAttribute)
            );
            throw new ArgumentException(message, nameof(metadata));
        }

        var selectList = new List<SelectListItem>();
        var groupList = new Dictionary<string, SelectListGroup>();
        foreach (var keyValuePair in metadata.EnumGroupedDisplayNamesAndValues)
        {
            var selectListItem = new SelectListItem
            {
                Text = keyValuePair.Key.Name,
                Value = keyValuePair.Value,
            };

            if (!string.IsNullOrEmpty(keyValuePair.Key.Group))
            {
                if (!groupList.TryGetValue(keyValuePair.Key.Group, out var group))
                {
                    group = new SelectListGroup() { Name = keyValuePair.Key.Group };
                    groupList[keyValuePair.Key.Group] = group;
                }

                selectListItem.Group = group;
            }

            selectList.Add(selectListItem);
        }

        return selectList;
    }

    private static string GetExpressionText(string expression)
    {
        // If it's exactly "model", then give them an empty string, to replicate the lambda behavior.
        return string.Equals(expression, "model", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : expression;
    }
}
