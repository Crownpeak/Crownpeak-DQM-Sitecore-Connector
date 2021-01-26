namespace Crownpeak.Sitecore.DQM.Logic
{
    public class CheckpointModel
    {
        public string IssueId { get; set; }

        public string Header { get; set; }

        public int IssueCount { get; set; }

        public bool CanHighligh { get; set; }

        public bool CanShowSource { get; set; }

        public string Description { get; set; }
    }
}