using Custom.Foundation.Forms.Extensions;
using Custom.Foundation.Forms.Services;
using Custom.Foundation.Xdb.Events;
using Sitecore.ExperienceForms.Mvc.Pipelines.ExecuteSubmit;
using Sitecore.Mvc.Pipelines;
using System;
using System.Linq;

namespace Custom.Foundation.Forms.Pipelines.ExecuteSubmit
{
    public class RegisterInteraction : MvcPipelineProcessor<ExecuteSubmitActionsEventArgs>
    {
        private IXdbContactService _service;

        public RegisterInteraction(IXdbContactService service)
        {
            _service = service;
        }

        public override void Process(ExecuteSubmitActionsEventArgs args)
        {
            try
            {
                DoProcess(args);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Warn("[RegisterInteraction]: ## Error", ex, this);
            }
        }

        public void DoProcess(ExecuteSubmitActionsEventArgs args)
        {
            var formEvent = new FormSubmission(DateTime.UtcNow) { FormId = args.FormSubmitContext.FormId };

            args.FormSubmitContext.Fields.ToList().ForEach(f => formEvent.CustomValues.Add(f.ItemId, f.GetStringValue()));

            _service.RegisterInteractionEvent(formEvent);
        }
    }
}