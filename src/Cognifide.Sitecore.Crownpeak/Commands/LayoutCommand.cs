using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Crownpeak.Sitecore.DQM.Commands
{
    public abstract class LayoutCommand : Command
    {
        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }

            var item = context.Items[0];
            if (item == null || !item.Paths.IsContentItem || item.Visualization.Layout == null)
            {
                return CommandState.Hidden;
            }

            return base.QueryState(context);
        }
    }
}