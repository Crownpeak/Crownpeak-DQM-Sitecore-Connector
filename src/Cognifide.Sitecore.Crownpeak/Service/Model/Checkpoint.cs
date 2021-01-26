using System;

namespace Crownpeak.Sitecore.DQM.Service.Model
{
    public class Checkpoint
    {
        public bool Failed { get; set; }
        public Highlight CanHighlight { get; set; }
        public string Name { get; set; }
        public bool Priority { get; set; }
        public string Reference { get; set; }
        public int Number { get; set; }
        public int CategoryNumber { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public class Highlight
        {
            public bool Page { get; set; }
            public bool Source { get; set; }
        }
    }
}
