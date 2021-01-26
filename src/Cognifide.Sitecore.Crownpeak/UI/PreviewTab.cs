using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Crownpeak.Sitecore.DQM.Logic;
using Crownpeak.Sitecore.DQM.Service.Model;
using HtmlAgilityPack;
using Sitecore.Data;
using Sitecore.Jobs;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Crownpeak.Sitecore.DQM.UI
{
    public class PreviewTab : BaseTab
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Response.SetAttribute("EEButton", "onclick",
                $"javascript:window.top.location.href='/?sc_mode=edit&amp;sc_itemid={WebUtil.GetQueryString("id")}';");
            if (WebUtil.GetQueryString("dqmee", "0").Equals("0"))
            {
                Response.SetStyle("EEHeader", "display", "none");
            }
        }

        [HandleMessage("preview:start")]
        protected void OnJobStart(Message message)
        {
            var uri = ItemUri.ParseQueryString();
            if (uri != null)
            {
                var validator = new ItemValidator { ItemUri = uri, Cookies = HttpContext.Current.Request.Cookies };
                Monitor.Start("Preview", "Crownpeak", new JobWorker(validator).GetPreview);
            }
        }

        [HandleMessage("preview:progress")]
        protected void OnJobProgress(Message message)
        {
            var job = JobManager.GetJob(Monitor.JobHandle);
            Response.SetInnerHtml("Indicator", $"Step {job.Status.Processed} of {job.Status.Total}...");
        }

        [HandleMessage("preview:fail")]
        protected void OnJobFail(Message message)
        {
            Response.SetStyle("Progress", "display", "none");
            Response.SetStyle("PreviewResult", "display", string.Empty);

            Response.SetInnerHtml("PreviewStatus", Renderer.RenderText("Error", message["error"] ?? string.Empty));
            Response.SetAttribute("PreviewContainer", "class", "scFixSizeNested");
        }

        [HandleMessage("preview:success")]
        protected void OnJobSuccess(Message message)
        {
            var job = JobManager.GetJob(Monitor.JobHandle);
            Response.SetStyle("Progress", "display", "none");
            Response.SetStyle("PreviewResult", "display", string.Empty);

            var args = job.Status.Result as ValidationArgs;
            if (args != null)
            {
                if (args.List.All(x => !x.Failed))
                {
                    Response.SetStyle("Success", "display", "block");
                    Response.SetAttribute("PreviewContainer", "style", "");
                }
                else
                {
                    var preview = Preprocess(args.Preview, args.List);

                    Response.SetInnerHtml("PreviewContainer", Renderer.RenderPreview(preview.Item1, preview.Item2));
                    Response.Eval("startup();");

                }
            }
            else
            {
                Response.SetInnerHtml("PreviewStatus", Renderer.RenderText("Information", "There is no content to validate"));
            }
            Response.SetAttribute("PreviewContainer", "class", "scFixSizeNested");
        }

        public static Tuple<string, CheckpointModel[]> Preprocess(string preview, Checkpoint[] argsList)
        {
            var checkpointModels = argsList.Where(x => x.Failed).Select(x => new CheckpointModel
            {
                IssueId = x.Id,
                Header = $"{x.Name}",
                CanHighligh = x.CanHighlight.Page,
                CanShowSource = x.CanHighlight.Source,
                IssueCount = 0,
                Description = x.Description
            }).ToDictionary(x => x.IssueId);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(preview);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//*[string(@data-dqm-id)]") ?? new HtmlNodeCollection(null);

            foreach (var node in nodes)
            {
                ClearHighlightStyle(node);

                foreach (var dqmId in GetDQMIds(node))
                {
                    AddHighlightClass(node, dqmId);

                    checkpointModels[dqmId].IssueCount += 1;
                }
            }

            // adding highlight style
            htmlDoc.DocumentNode.SelectSingleNode("//head").AppendChild(HtmlNode.CreateNode(string.Format(@"<style>
[class*={0}] {{
    background: yellow;
    border: 2px solid #ff0000
}}

@keyframes dqmBlinkKeyframes {{
    0%   {{opacity: 0;}}    
    50%   {{opacity: 1;}}    
    100%  {{opacity: 0;}}    
}}

.dqmBlink {{
    background-color: yellow;
    animation-name: dqmBlinkKeyframes;
    animation-duration: 750ms;
    animation-iteration-count: infinite;
    animation-timing-function: linear;
    animation-iteration-count: 3
}} 
</style>", _highlightClass)));

            var builder = new StringBuilder();
            htmlDoc.DocumentNode.WriteContentTo(new StringWriter(builder));

            return Tuple.Create(builder.ToString(), checkpointModels.Values.ToArray());
        }

        private static IEnumerable<string> GetDQMIds(HtmlNode node)
        {
            return node.Attributes["data-dqm-id"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        }

        private static void AddHighlightClass(HtmlNode node, string dqmId)
        {
            node.SetAttributeValue("class", node.GetAttributeValue("class", "") + " " + _highlightClass + dqmId);
        }

        private static void ClearHighlightStyle(HtmlNode node)
        {
            CssStyleCollection style = new Panel().Style;
            style.Value = node.GetAttributeValue("style", "");
            if (style["background"] == "yellow")
                style.Remove("background");
            if (style["border"] == "2px solid #ff0000")
                style.Remove("border");
            node.SetAttributeValue("style", style.Value ?? string.Empty);
        }

        private static string _highlightClass = "dqmPreviewHighlight";
    }
}