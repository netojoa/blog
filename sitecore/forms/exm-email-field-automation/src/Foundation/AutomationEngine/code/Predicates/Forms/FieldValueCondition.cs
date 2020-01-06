using Custom.Foundation.Xdb.Events;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;

namespace Custom.Foundation.AutomationEngine.Predicates.Forms
{
    public class FieldValueCondition : ICondition, IMappableRuleEntity
    {
        public string FieldId { get; set; }

        public StringOperationType Comparison { get; set; }

        public string Value { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            try
            {
                return DoEvaluate(context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool DoEvaluate(IRuleExecutionContext context)
        {
            Condition.Requires<IRuleExecutionContext>(context, nameof(context)).IsNotNull<IRuleExecutionContext>();
            Contact contact = context.Fact<Contact>((string)null);
            return contact?.Interactions != null && this.IsMatch(contact);
        }

        protected virtual bool IsMatch(Contact contact)
        {
            Condition.Requires<Contact>(contact, nameof(contact)).IsNotNull<Contact>();

            var lastFormSubmission = contact.Interactions
                .Where(i => i.Events.Any(f => f.DefinitionId == FormSubmission.EventDefinitionId))
                .OrderByDescending(i => i.EndDateTime)
                .FirstOrDefault();

            return lastFormSubmission
                .Events
                .Any(e => e.CustomValues.Any(s => CompareGuids(s.Key, FieldId) && this.Comparison.Evaluate(s.Value, Value)));
        }

        private bool CompareGuids(string left, string right)
        {
            Guid leftGuid;

            if (Guid.TryParse(left, out leftGuid) == false)
                return false;

            Guid rightGuid;
            if (Guid.TryParse(right, out rightGuid) == false)
                return false;

            return leftGuid == rightGuid;
        }
    }
}