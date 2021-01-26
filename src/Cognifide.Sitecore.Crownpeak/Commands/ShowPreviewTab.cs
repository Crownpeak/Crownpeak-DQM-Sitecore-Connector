using System.Text.RegularExpressions;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Resources;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Framework.Scripts;
using Sitecore.Web.UI.Sheer;

namespace Crownpeak.Sitecore.DQM.Commands
{
    public class ShowPreviewTab : LayoutCommand
    {
        private readonly Regex _previewTabRegex = new Regex("CrownpeakPreviewTab_[0-9a-f]{32}", RegexOptions.IgnoreCase);

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return;
            }

            var tabId = $"CrownpeakPreviewTab_{context.Items[0].ID.Guid:N}";
            var editorTabs = WebUtil.GetFormValue("scEditorTabs");

            if (ShouldShowNewTab(editorTabs, tabId))
            {
                CloseOtherPreviewTabs(editorTabs);
                Show(context, tabId);
            }

            ActivateTab(tabId);
        }

        private static bool ShouldShowNewTab(string editorTabs, string tabId)
        {
            return !editorTabs.Contains(tabId);
        }

        private void CloseOtherPreviewTabs(string editorTabs)
        {
            foreach (Match match in _previewTabRegex.Matches(editorTabs))
            {
                SheerResponse.Eval($"scContent.closeEditorTab('{match.Value}')");
            }
        }

        private static void Show(CommandContext context, string tabId)
        {
            var urlString = new UrlString();
            context.Items[0].Uri.AddToUrlString(urlString);
            UIUtil.AddContentDatabaseParameter(urlString);

            SheerResponse.Eval(new ShowEditorTab
                {
                    Command = "crownpeakdqm:previewtab",
                    Header = "Crownpeak DQM Preview",
                    Icon = Images.GetThemedImageSource("/Content/crownpeak/16x16/crownpeak.png"),
                    Url = UIUtil.GetUri("control:Crownpeak.PreviewTab", urlString),
                    Id = tabId,
                    Closeable = true,
                }.ToString());
        }

        private static void ActivateTab(string tabId)
        {
            SheerResponse.Eval($"scContent.onEditorTabClick(null, null, '{tabId}')");
        }
    }
}
