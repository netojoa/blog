using Microsoft.Extensions.Logging;
using Sitecore.Framework.Publishing.PublishJobQueue;
using Sitecore.Framework.Scheduling;
using Sitecore.Framework.Scheduling.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandbox.Customization.Publishing.Tasks
{

    [DefaultId("AutoPublishTask")]
    public class AutoPublishTask : TaskDefinition
    {
        private readonly IPublishJobQueueService _queueService;
        private readonly ILogger<AutoPublishTask> _logger;

        public string ContextLanguage { get; set; }
        public string Languages { get; set; }
        public string Targets { get; set; }
        public string Source { get; set; }
        public bool Descendants { get; set; }
        public bool RelatedItems { get; set; }
        public string User { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public override List<string> Categories { get => new List<string>() { "Sandbox", "AutoPublishing" }; }

        public AutoPublishTask(IPublishJobQueueService queueService, ILogger<AutoPublishTask> logger)
        {
            this._logger = logger;
            this._queueService = queueService;
            this._logger.LogWarning("###################### Auto Publish Job has been instantiated");
        }

        protected override Task OnExecute(IScheduledTaskExecutionContext context)
        {
            this._logger.LogWarning("###################### Auto Publish Job started");
            var publishOptions = GetPublishOptions();
            return _queueService.QueueJob(publishOptions);
        }

        private PublishOptions GetPublishOptions()
        {

            this._logger.LogWarning("###################### Reading PublishOptions");

            var languages = this.Languages.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var targets = this.Targets.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            return new PublishOptions(
                contextLanguage: this.ContextLanguage,
                languages: languages,
                targets: targets,
                source: this.Source,
                descendants: this.Descendants,
                relatedItems: this.RelatedItems,
                itemId: null,
                user: this.User,
                metadata: this.Metadata
           );
        }
    }
}