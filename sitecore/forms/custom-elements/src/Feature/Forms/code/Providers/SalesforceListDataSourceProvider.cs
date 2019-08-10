namespace Custom.Feature.Forms.Providers
{
    using Sitecore.Data.Items;
    using Sitecore.ExperienceForms.Mvc.DataSource;
    using Sitecore.ExperienceForms.Mvc.Models;
    using System;
    using System.Collections.Generic;

    // Disclaimer: This is an example that only applies to Sitecore 9.0.x.
    public class SalesforceListDataSourceProvider : IListDataSourceProvider
    {
        public IEnumerable<Item> GetDataItems(string dataSource)
        {
            // This method is supposed to be called by GetListItems. We don't need to implement it.
            throw new NotImplementedException();
        }

        public IEnumerable<ListFieldItem> GetListItems(string dataSource, string displayFieldName, string valueFieldName, string defaultSelection)
        {
            try
            {
                return GetSalesforceItems(defaultSelection);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SalesforceListDataSourceProvider:: Error getting Salesforce pick list values", ex, this);
                return new List<ListFieldItem>();
            }
        }

        protected IEnumerable<ListFieldItem> GetSalesforceItems(string defaultSelection)
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