using System;
using System.Collections.Generic;
using Sitecore.Jobs.AsyncUI;

namespace Crownpeak.Sitecore.DQM.Logic
{
    public class JobWorker
    {
        private readonly AbstractValidator _validator;

        public JobWorker(AbstractValidator validator)
        {
            _validator = validator;
        }

        public void GetSource()
        {
            RunTasks("source", new Action<ValidationArgs>[] { args => _validator.GetHighlights(args, true) });
        }

        public void GetPreview()
        {
            RunTasks("preview", new Action<ValidationArgs>[] { _validator.GetCheckpoints, args => _validator.GetHighlights(args, false) });
        }

        private void RunTasks(string messageSuffix, IList<Action<ValidationArgs>> actions)
        {
            try
            {
                var args = new ValidationArgs();
                JobContext.Job.Status.Result = args;
                JobContext.Job.Status.Total = 3 + actions.Count;

                ReportProgress(messageSuffix);
                _validator.GetContent(args);

                if (!string.IsNullOrWhiteSpace(args.Content))
                {
                    ReportProgress(messageSuffix);
                    _validator.SendContent(args);

                    foreach (var action in actions)
                    {
                        ReportProgress(messageSuffix);
                        action(args);
                    }

                    ReportProgress(messageSuffix);
                    _validator.Cleanup(args);
                }

                JobContext.SendMessage($"{messageSuffix}:success");
            }
            catch (Exception ex)
            {
                JobContext.SendMessage($"{messageSuffix}:fail(error=" + ex.Message + ")");
            }
        }

        private static void ReportProgress(string messageSuffix)
        {
            JobContext.Job.Status.Processed++;
            JobContext.PostMessage($"{messageSuffix}:progress");
        }
    }
}
