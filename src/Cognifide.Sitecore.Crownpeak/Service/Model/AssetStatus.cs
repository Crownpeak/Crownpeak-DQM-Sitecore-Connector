using System;

namespace Crownpeak.Sitecore.DQM.Service.Model
{
    public class AssetStatus
    {
        public string AssetId { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
        public string SiteName { get; set; }
        public int TotalCheckpoints { get; set; }
        public int TotalErrors { get; set; }
        public Checkpoint[] Checkpoints { get; set; }
    }
}
