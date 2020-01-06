using Custom.Foundation.Forms.Extensions;
using Custom.Foundation.Forms.Models;
using Custom.Foundation.Forms.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using System;

namespace Custom.Foundation.Forms.SubmitActions
{
    public class IdentifyContact : SubmitActionBase<IdentifyContactActionData>
    {
        public const string DefaultIdentifierSource = "Forms";

        private IXdbContactService _service;

        public IdentifyContact(ISubmitActionData submitActionData) : this(submitActionData,
             ServiceLocator.ServiceProvider.GetService<IXdbContactService>())
        {
        }

        public IdentifyContact(ISubmitActionData submitActionData, IXdbContactService service) : base(submitActionData)
        {
            _service = service;
        }

        protected override bool Execute(IdentifyContactActionData data, FormSubmitContext formSubmitContext)
        {
            try
            {
                return DoExecute(data, formSubmitContext);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Warn("[RegisterInteraction]: ## Error", ex, this);
                return true;
            }
        }

        protected bool DoExecute(IdentifyContactActionData data, FormSubmitContext formSubmitContext)
        {
            if (data.IdentifierFieldId == null || data.IdentifierFieldId == Guid.Empty)
            {
                Sitecore.Diagnostics.Log.Warn($"Field {nameof(data.IdentifierFieldId)} is empty.", this);
                return true;
            }

            var identifierField = formSubmitContext.Fields.ById(data.IdentifierFieldId.Value);

            _service.IdentifyCurrentAndUpdateEmailFacet(DefaultIdentifierSource, identifierField.GetStringValue());

            return true;
        }
    }
}