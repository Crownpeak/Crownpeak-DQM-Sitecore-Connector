using System;
using Crownpeak.Sitecore.DQM.Logic;
using Sitecore;
using Sitecore.Jobs.AsyncUI;
using Sitecore.Web.UI.Sheer;

namespace Crownpeak.Sitecore.DQM.UI
{
    public abstract class BaseTab : BaseForm
    {
        protected JobMonitor Monitor { get; set; }
        protected readonly HtmlRenderer Renderer = new HtmlRenderer();
        protected ClientResponse Response { get { return Context.ClientPage.ClientResponse; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Context.ClientPage.IsEvent)
            {
                if (Monitor == null)
                {
                    Monitor = new JobMonitor();
                    Monitor.ID = "Monitor";
                    Context.ClientPage.Controls.Add(Monitor);
                }
            }
            else if (Monitor == null)
            {
                Monitor = (JobMonitor)Context.ClientPage.FindControl("Monitor");
            }
        }
    }
}
