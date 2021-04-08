using System;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using Crownpeak.Sitecore.DQM.Service;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Crownpeak.Sitecore.DQM.Logic
{
    [Serializable]
    public class ItemValidator : AbstractValidator
    {
        private HttpCookieCollection _cookies;

        public override string Name
        {
            get { return "Crownpeak item validator"; }
        }

        public ItemValidator()
        {
        }

        public ItemValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected override string GetErrorText(int totalErrors, int totalCheckpoints)
        {
            return GetText(
                "The page represented by the item '{0}' does not meat {1} out of {2} requirements defined by Crownpeak",
                GetItem().Paths.ContentPath, totalErrors.ToString(), totalCheckpoints.ToString());
        }

        public override void GetContent(ValidationArgs args)
        {
            Item item;
            if (ItemUri == null || (item = Database.GetItem(ItemUri)) == null)
            {
                throw new ApplicationException("The item does not exist.");
            }
            if (!item.Paths.IsContentItem)
            {
                throw new ApplicationException("The item is not a content item.");
            }
            if (item.Visualization.Layout == null)
            {
                throw new ApplicationException("The item does not have layout defined.");
            }

            try
            {
                args.Content = ItemContentHelper.GetContent(item, Cookies);
            }
            catch (WebException ex)
            {
                throw new ApplicationException("The page failed to render properly: " + ex.Message, ex);
            }
        }

        public HttpCookieCollection Cookies
        {
            get { return _cookies ?? (_cookies = HttpContext.Current.Request.Cookies); }
            set { _cookies = value; }
        }
    }

}
