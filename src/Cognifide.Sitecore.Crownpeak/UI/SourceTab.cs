using System.Linq;
using System.Web;
using Crownpeak.Sitecore.DQM.Logic;
using Sitecore.Data;
using Sitecore.Jobs;
using Sitecore.Web.UI.Sheer;

namespace Crownpeak.Sitecore.DQM.UI
{
    public class SourceTab : BaseTab
    {
        public override void HandleMessage(Message message)
        {
            if (message.Name == "event:click")
                Response.CloseWindow();

            base.HandleMessage(message);
        }

        [HandleMessage("source:start")]
        protected void OnJobStart(Message message)
        {
            var uri = ItemUri.ParseQueryString();
            if (uri != null)
            {
                var checkpointId = HttpContext.Current.Request.QueryString["checkpointId"];
                var validator = new ItemValidator { ItemUri = uri, CheckpointId = checkpointId, Cookies = HttpContext.Current.Request.Cookies };
                Monitor.Start("Source", "Crownpeak", new JobWorker(validator).GetSource);
            }
        }

        [HandleMessage("source:progress")]
        protected void OnJobProgress(Message message)
        {
            var job = JobManager.GetJob(Monitor.JobHandle);
            Response.SetInnerHtml("Indicator", $"Step {job.Status.Processed} of {job.Status.Total}...");
        }

        [HandleMessage("source:fail")]
        protected void OnJobFail(Message message)
        {
            Response.SetStyle("Progress", "display", "none");
            Response.SetStyle("Result", "display", string.Empty);

            Response.SetInnerHtml("Status", Renderer.RenderText("Error", message["error"] ?? string.Empty));
            Response.SetAttribute("Container", "class", "scFixSizeNested");
        }

        [HandleMessage("source:success")]
        protected void OnJobSuccess(Message message)
        {
            var job = JobManager.GetJob(Monitor.JobHandle);
            Response.SetStyle("Progress", "display", "none");
            Response.SetStyle("Result", "display", string.Empty);

            var args = job.Status.Result as ValidationArgs;
            if (args != null)
            {
                Response.SetInnerHtml("Container", HtmlRenderer.RenderIFrame(args.Source.First(),
                    inlineStyle: "width:100%;height:100%;display:block"));
            }
            else
            {
                Response.SetInnerHtml("Status", Renderer.RenderText("Information", "There is no content to validate"));
            }

            Response.SetAttribute("Container", "class", "scFixSizeNested");
        }
    }
}