namespace Custom.Feature.Forms.Models.Fields
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.ExperienceForms.Mvc.Models.Fields;
    using System;

    [Serializable]
    public class FieldsetViewModel : TitleFieldViewModel
    {

        private const string IsLabelVisibleFieldName = "Is Label Visible";

        public bool IsLabelVisible { get; set; }

        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
            this.IsLabelVisible = MainUtil.GetBool(item.Fields[IsLabelVisibleFieldName]?.Value, false);
        }

        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
            item.Fields[IsLabelVisibleFieldName]?.SetValue(IsLabelVisible ? "1" : "0", true);
        }
    }
}