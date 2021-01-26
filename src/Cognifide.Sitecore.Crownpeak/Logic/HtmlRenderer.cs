using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;

namespace Crownpeak.Sitecore.DQM.Logic
{
    public class HtmlRenderer
    {
        public string RenderText(string title, string help)
        {
            var writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("class", "scMessageBarIcon");
            writer.RenderBeginTag("div");
            writer.RenderEndTag();

            writer.AddAttribute("class", "scMessageBarTextContainer");
            writer.RenderBeginTag("div");
            writer.AddAttribute("class", "scMessageBarTitle");
            writer.RenderBeginTag("div");
            writer.Write(title);
            writer.RenderEndTag();

            if (!string.IsNullOrEmpty(help))
            {
                writer.AddAttribute("class", "scMessageBarText");
                writer.RenderBeginTag("div");
                writer.Write(help);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();

            return writer.InnerWriter.ToString();
        }

        public string RenderPreview(string content, CheckpointModel[] checkpoints)
        {
            var frameId = "_" + Guid.NewGuid().ToString("N");
            return string.IsNullOrWhiteSpace(content) ? string.Empty : GetPreviewJs(checkpoints, frameId) + RenderIFrame(content, frameId);
        }

        private string GetPreviewJs(CheckpointModel[] checkpoints, string frameId)
        {
            return @"
                <div id=""container"">
                        <div class=""dqmlist"" ></div>
                </div>
            <script type=""text/javascript"">
    window.dqmPreviewFrame = '" + frameId + @"';
    window.checkpoints = " + JsonConvert.SerializeObject(checkpoints) + "</script>";
        }

        public static string RenderIFrame(string html, string frameId = null,
            string inlineStyle = "")
        {
            var writer = new HtmlTextWriter(new StringWriter());

            var id = frameId ?? "_" + Guid.NewGuid().ToString("N");
            writer.AddAttribute("id", id);
            writer.AddAttribute("style", "border:none;" + inlineStyle);

            writer.RenderBeginTag("iframe");
            writer.RenderEndTag();

            writer.AddAttribute("type", "text/javascript");
            writer.RenderBeginTag("script");
            writer.WriteLine("var {0} = document.getElementById('{0}');", id);
            writer.WriteLine("{0}.contentWindow.document.open();", id);
            writer.WriteLine("{0}.contentWindow.document.write(\"{1}\");", id, HttpUtility.JavaScriptStringEncode(html));
            writer.WriteLine("{0}.contentWindow.document.close();", id);
            writer.RenderEndTag();

            return writer.InnerWriter.ToString();
        }

    }
}
