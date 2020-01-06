using Sitecore.XConnect;
using System;

namespace Custom.Foundation.Xdb.Events
{
    public class FormSubmission : Event
    {
        public static Guid EventDefinitionId { get; } = new Guid("8d131154-dc93-43cf-9901-655b33ca5e87");

        public Guid FormId { get; set; }

        public FormSubmission(DateTime timestamp) : base(FormSubmission.EventDefinitionId, timestamp)
        {
        }
    }
}