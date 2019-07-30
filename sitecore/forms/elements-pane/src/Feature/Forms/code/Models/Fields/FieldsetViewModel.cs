using System;
using Sitecore.Data.Items;
using Sitecore.ExperienceForms.Mvc.Models.Fields;

namespace Custom.Feature.Forms.Models.Fields
{

    [Serializable]
    public class FieldsetViewModel : TitleFieldViewModel
    {
        protected override void InitItemProperties(Item item)
        {
            base.InitItemProperties(item);
        }

        protected override void UpdateItemFields(Item item)
        {
            base.UpdateItemFields(item);
        }
    }
}