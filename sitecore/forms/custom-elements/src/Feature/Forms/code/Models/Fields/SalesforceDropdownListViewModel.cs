namespace Custom.Feature.Forms.Models.Fields
{
    using Custom.Feature.Forms.Providers;
    using Sitecore.ExperienceForms.FieldSettings;
    using Sitecore.ExperienceForms.Mvc.Models;
    using Sitecore.ExperienceForms.Mvc.Models.Fields;
    using System;

    [Serializable]
    public class SalesforceDropdownListViewModel : DropDownListViewModel
    {
        protected override IFieldSettingsManager<ListFieldItemCollection> DataSourceSettingsManager => new SalesforceSettingsManager();
    }
}