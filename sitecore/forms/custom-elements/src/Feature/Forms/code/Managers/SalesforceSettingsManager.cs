namespace Custom.Feature.Forms.Providers
{
    using Sitecore.Data.Items;
    using Sitecore.ExperienceForms.FieldSettings;
    using Sitecore.ExperienceForms.Mvc.Models;
    using System;

    [Serializable]
    public class SalesforceSettingsManager : IFieldSettingsManager<ListFieldItemCollection>
    {
        public ListFieldItemCollection GetSettings(Item fieldItem)
        {
            try
            {
                return GetSalesforceItems();
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SalesforceListDataSourceProvider:: Error getting Salesforce pick list values", ex, this);
                return new ListFieldItemCollection();
            }
        }

        public void SaveSettings(Item fieldItem, ListFieldItemCollection settings)
        {
            // This method is supposed to create items in the content tree from the specified datasource. We don't need it.
        }

        protected ListFieldItemCollection GetSalesforceItems()
        {
            // Add some logic here to call a service that gets data from Salesforce and converts it to a ListFieldItemCollection

            var items = new ListFieldItemCollection();

            items.AddRange(new ListFieldItem[] {
                new ListFieldItem { Value = "Salesforce Value 1", Text = "Salesforce Item 1" },
                new ListFieldItem { Value = "Salesforce Value 1", Text = "Salesforce Item 2" },
                new ListFieldItem { Value = "Salesforce Value 1", Text = "Salesforce Item 3" },
                new ListFieldItem { Value = "Salesforce Value 1", Text = "Salesforce Item 4" }
            });

            return items;
        }
    }
}