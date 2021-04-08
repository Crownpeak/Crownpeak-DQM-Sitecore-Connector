using System;

namespace Crownpeak.Sitecore.DQM.Service.Model
{
    public class Asset
    {
        public string WebsiteId { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public DateTime Expires { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
