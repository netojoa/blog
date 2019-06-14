using Bootstrap.Foundation.Forms.Extensions;
using HtmlAgilityPack;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderField;
using Sitecore.Mvc.Pipelines;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bootstrap.Foundation.Forms.Pipelines.RenderField
{
    public class BootstrapFieldsProcessor : MvcPipelineProcessor<RenderFieldEventArgs>
    {
        private const string FormGroupClass = " form-group";
        private const string FormControlClass = " form-control";
        private const string FormLabelClass = " form-label";
        private const string HelpBlock = " help-block";

        private readonly List<string> fieldTypes = new List<string>();

        public void AddField(string fieldId)
        {
            this.fieldTypes.Add(fieldId);
        }

        private bool isEditMode = false;

        public override void Process(RenderFieldEventArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.ViewModel == null) return;

            bool isFieldTypeIncluded = this.fieldTypes.Contains(args.RenderingSettings.FieldTypeId);
            if (!isFieldTypeIncluded)
                return;

            this.isEditMode = (args.FormBuilderContext.FormBuilderMode != FormBuilderMode.Load);

            string htmlString = BootstrapFields(args.Result.ToHtmlString());
            args.Result = new MvcHtmlString(htmlString);
        }

        protected string BootstrapFields(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            AppendFormLabelClass(htmlDoc);
            AppendFormControlClass(htmlDoc);
            AppendHelpBlockClass(htmlDoc);

            if (this.isEditMode)
            {
                WrapFormGroupToEditField(htmlDoc);
            }
            else
            {
                WrapFormGroup(htmlDoc);
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }

        private void AppendHelpBlockClass(HtmlDocument document)
        {
            var helpBlockNodes = document.DocumentNode.SelectNodes("//*[contains(@class,'field-validation-valid')]");
            if (helpBlockNodes != null)
            {
                foreach (var fieldElement in helpBlockNodes)
                    fieldElement.Attributes.AppendOrCreate("class", HelpBlock);
            }
        }

        protected void WrapFormGroupToEditField(HtmlDocument document)
        {
            var nodes =
                document.DocumentNode.SelectNodes("//*[contains(@class,'sc-formdesign-fieldcontainer')]");

            if (nodes == null) return;

            foreach (var fieldElement in nodes)
            {
                HtmlNode formGroupElement = document.CreateElement("div");
                formGroupElement.Attributes.AppendOrCreate("class", FormGroupClass);
                foreach (var elementChildNode in fieldElement.ChildNodes)
                {
                    formGroupElement.AppendChild(elementChildNode.Clone());
                }
                while (fieldElement.FirstChild != null)
                    fieldElement.RemoveChild(fieldElement.FirstChild);
                fieldElement.AppendChild(formGroupElement.Clone());
            }
        }

        protected void WrapFormGroup(HtmlDocument document)
        {
            HtmlNode formGroupElement = document.CreateElement("div");
            formGroupElement.Attributes.AppendOrCreate("class", FormGroupClass);
            formGroupElement.AppendChild(document.DocumentNode.Clone());
            document.LoadHtml(formGroupElement.OuterHtml);
        }

        protected void AppendFormLabelClass(HtmlDocument document)
        {
            var labelNodes = document.DocumentNode.SelectNodes("//label");
            if (labelNodes != null)
            {
                foreach (var fieldElement in labelNodes)
                    fieldElement.Attributes.AppendOrCreate("class", FormLabelClass);
            }
        }

        protected void AppendFormControlClass(HtmlDocument document)
        {
            var inputNodes = document.DocumentNode.SelectNodes("//input[@type='text']|//input[@type='email']|//input[@type='number']|//input[@type='tel']");
            if (inputNodes != null)
            {
                foreach (var fieldElement in inputNodes)
                    fieldElement.Attributes.AppendOrCreate("class", FormControlClass);
            }

            var textareaNodes = document.DocumentNode.SelectNodes("//textarea");
            if (textareaNodes != null)
            {
                foreach (var fieldElement in textareaNodes)
                    fieldElement.Attributes.AppendOrCreate("class", FormControlClass);
            }

            var selectNodes = document.DocumentNode.SelectNodes("//select");
            if (selectNodes != null)
            {
                foreach (var fieldElement in selectNodes)
                    fieldElement.Attributes.AppendOrCreate("class", FormControlClass);
            }
        }

    }
}