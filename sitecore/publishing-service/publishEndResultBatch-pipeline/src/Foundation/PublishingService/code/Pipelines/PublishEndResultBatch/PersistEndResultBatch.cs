using Sitecore;
using Sitecore.Publishing.Service.Pipelines.BulkPublishingEnd;
using System.Net.Mail;
using System.Linq;
using System;

namespace Sandbox.Foundation.PublishingService.Pipelines.PublishEndResultBatch
{
    public class PersistEndResultBatch
    {

        public string MailFrom { get; set; }
        public string MailTo { get; set; }

        public void Process(PublishEndResultBatchArgs args)
        {


            if (args.Aborted)
            {
                SendAbortMessage(args);
            }
            else
            {
                SendDetailsMessage(args);
            }

        }

        private void SendDetailsMessage(PublishEndResultBatchArgs args)
        {
            MailMessage message = new MailMessage(this.MailFrom, this.MailTo);
            message.Subject = $"Publishing job {args.JobData.JobId} finished at {DateTime.Now}";
            var itemsAffected = args.Batch.Select(b => b.EntityId).Distinct().Count();
            message.Body = "Finished publishing job!\n";
            message.Body += $"({itemsAffected}) items were published\n";
            message.Body += $"Date: {args.JobData.PublishDate}\n";
            message.Body += $"Job Id: {args.JobData.JobId}\n";
            message.Body += $"Username: {args.JobData.Username}\n";
            MainUtil.SendMail(message);
        }

        private void SendAbortMessage(PublishEndResultBatchArgs args)
        {
            MailMessage message = new MailMessage(this.MailFrom, this.MailTo);
            message.Subject = $"Publishing job {args.JobData.JobId} aborted at {DateTime.Now}";
            message.Body = $"A publishing job was just aborted. Job Id: {args.JobData.JobId} \n Message: {args.Message}";
            MainUtil.SendMail(message);
        }

    }
}