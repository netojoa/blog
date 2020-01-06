using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom.Foundation.Forms.Extensions
{
    public static class ViewModelExtensions
    {
        public static string GetStringValue(this IViewModel helper)
        {
            if (helper is InputViewModel<bool> boolValue)
                return boolValue.Value ? "1" : "0";

            if (helper is ListViewModel listView)
                return helper.GetListViewValue();

            if (helper is InputViewModel<string> stringValue)
                return stringValue.Value;

            if (helper is InputViewModel<DateTime> dateTimeValue)
                return dateTimeValue.Value.ToString();

            if (helper is InputViewModel<double?> doubleValue)
                return doubleValue.Value?.ToString();

            return string.Empty;
        }

        public static string GetListViewValue(this IViewModel helper)
        {
            if (helper is ListViewModel listField)
            {
                var selectedValue = listField.Items.SingleOrDefault(s => s.Selected);
                return selectedValue?.Value;
            }

            return string.Empty;
        }

        public static string GetListViewText(this IViewModel helper)
        {
            if (helper is ListViewModel listField)
            {
                var selectedValue = listField.Items.SingleOrDefault(s => s.Selected);
                return selectedValue?.Text;
            }

            return string.Empty;
        }

        public static IViewModel ById(this IEnumerable<IViewModel> fields, Guid id)
        {
            return fields.FirstOrDefault(f => Guid.Parse(f.ItemId) == id);
        }
    }
}