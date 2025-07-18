// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Represents a list that lets users select multiple items.
/// This class is typically rendered as an HTML <c>&lt;select multiple="multiple"&gt;</c> element with the specified collection
/// of <see cref="SelectListItem"/> objects.
/// </summary>
public class MultiSelectList : IEnumerable<SelectListItem>
{
    private readonly IList<SelectListGroup> _groups;
    private IList<SelectListItem> _selectListItems;

    /// <summary>
    /// Initialize a new instance of <see cref="MultiSelectList"/>.
    /// </summary>
    /// <param name="items">The items.</param>
    public MultiSelectList(IEnumerable items)
        : this(items, selectedValues: null)
    {
        ArgumentNullException.ThrowIfNull(items);
    }

    /// <summary>
    /// Initialize a new instance of <see cref="MultiSelectList"/>.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="selectedValues">The selected values.</param>
    public MultiSelectList(IEnumerable items, IEnumerable selectedValues)
        : this(items, dataValueField: null, dataTextField: null, selectedValues: selectedValues)
    {
        ArgumentNullException.ThrowIfNull(items);
    }

    /// <summary>
    /// Initialize a new instance of <see cref="MultiSelectList"/>.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="dataValueField">The data value field.</param>
    /// <param name="dataTextField">The data text field.</param>
    public MultiSelectList(IEnumerable items, string dataValueField, string dataTextField)
        : this(items, dataValueField, dataTextField, selectedValues: null)
    {
        ArgumentNullException.ThrowIfNull(items);
    }

    /// <summary>
    /// Initialize a new instance of <see cref="MultiSelectList"/>.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="dataValueField">The data value field.</param>
    /// <param name="dataTextField">The data text field.</param>
    /// <param name="selectedValues">The selected values.</param>
    public MultiSelectList(
        IEnumerable items,
        string dataValueField,
        string dataTextField,
        IEnumerable selectedValues
    )
        : this(items, dataValueField, dataTextField, selectedValues, dataGroupField: null)
    {
        ArgumentNullException.ThrowIfNull(items);
    }

    /// <summary>
    /// Initializes a new instance of the MultiSelectList class by using the items to include in the list,
    /// the data value field, the data text field, the selected values, and the data group field.
    /// </summary>
    /// <param name="items">The items used to build each <see cref="SelectListItem"/> of the list.</param>
    /// <param name="dataValueField">The data value field. Used to match the Value property of the corresponding
    /// <see cref="SelectListItem"/>.</param>
    /// <param name="dataTextField">The data text field. Used to match the Text property of the corresponding
    /// <see cref="SelectListItem"/>.</param>
    /// <param name="selectedValues">The selected values field. Used to match the Selected property of the
    /// corresponding <see cref="SelectListItem"/>.</param>
    /// <param name="dataGroupField">The data group field. Used to match the Group property of the corresponding
    /// <see cref="SelectListItem"/>.</param>
    public MultiSelectList(
        IEnumerable items,
        string dataValueField,
        string dataTextField,
        IEnumerable selectedValues,
        string dataGroupField
    )
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
        DataValueField = dataValueField;
        DataTextField = dataTextField;
        SelectedValues = selectedValues;
        DataGroupField = dataGroupField;

        if (DataGroupField != null)
        {
            _groups = new List<SelectListGroup>();
        }
    }

    /// <summary>
    /// Gets the data group field.
    /// </summary>
    public string DataGroupField { get; }

    /// <summary>
    /// Gets the data text field.
    /// </summary>
    public string DataTextField { get; }

    /// <summary>
    /// Gets the data value field.
    /// </summary>
    public string DataValueField { get; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IEnumerable Items { get; }

    /// <summary>
    /// Gets the selected values.
    /// </summary>
    public IEnumerable SelectedValues { get; }

    /// <inheritdoc />
    public virtual IEnumerator<SelectListItem> GetEnumerator()
    {
        if (_selectListItems == null)
        {
            _selectListItems = GetListItems();
        }

        return _selectListItems.GetEnumerator();
    }

    private IList<SelectListItem> GetListItems()
    {
        return (!string.IsNullOrEmpty(DataValueField))
            ? GetListItemsWithValueField()
            : GetListItemsWithoutValueField();
    }

    private IList<SelectListItem> GetListItemsWithValueField()
    {
        var selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (SelectedValues != null)
        {
            foreach (var value in SelectedValues)
            {
                var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
                selectedValues.Add(stringValue);
            }
        }

        var listItems = new List<SelectListItem>();
        foreach (var item in Items)
        {
            var value = Eval(item, DataValueField);
            var newListItem = new SelectListItem
            {
                Group = GetGroup(item),
                Value = value,
                Text = Eval(item, DataTextField),
                Selected = selectedValues.Contains(value),
            };

            listItems.Add(newListItem);
        }

        return listItems;
    }

    private IList<SelectListItem> GetListItemsWithoutValueField()
    {
        var selectedValues = new HashSet<object>();
        if (SelectedValues != null)
        {
            selectedValues.UnionWith(SelectedValues.Cast<object>());
        }

        var listItems = new List<SelectListItem>();
        foreach (var item in Items)
        {
            var newListItem = new SelectListItem
            {
                Group = GetGroup(item),
                Text = Eval(item, DataTextField),
                Selected = selectedValues.Contains(item),
            };

            listItems.Add(newListItem);
        }

        return listItems;
    }

    private static string Eval(object container, string expression)
    {
        var value = container;
        if (!string.IsNullOrEmpty(expression))
        {
            var viewDataInfo = ViewDataEvaluator.Eval(container, expression);
            value = viewDataInfo.Value;
        }

        return Convert.ToString(value, CultureInfo.CurrentCulture);
    }

    private SelectListGroup GetGroup(object container)
    {
        if (_groups == null)
        {
            return null;
        }

        var groupName = Eval(container, DataGroupField);
        if (string.IsNullOrEmpty(groupName))
        {
            return null;
        }

        // We use StringComparison.CurrentCulture because the group name is used to display as the value of
        // optgroup HTML tag's label attribute.
        SelectListGroup group = null;
        for (var index = 0; index < _groups.Count; index++)
        {
            if (string.Equals(_groups[index].Name, groupName, StringComparison.CurrentCulture))
            {
                group = _groups[index];
                break;
            }
        }

        if (group == null)
        {
            group = new SelectListGroup() { Name = groupName };
            _groups.Add(group);
        }

        return group;
    }

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
