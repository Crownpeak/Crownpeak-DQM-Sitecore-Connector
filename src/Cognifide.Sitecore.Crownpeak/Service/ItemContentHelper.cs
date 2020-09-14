using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Cognifide.Sitecore.Crownpeak.Logic;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Web;

namespace Cognifide.Sitecore.Crownpeak.Service
{
    public class ItemContentHelper
    {
        internal static string GetContent(Item item, HttpCookieCollection cookies)
        {
            return GetContent(GetUrl(item), cookies);
        }

        public static string GetUrl(Item item)
        {
            var defaultOptions = LinkManager.GetDefaultUrlBuilderOptions();

            var siteInfo = SiteHelper.GetSiteInfo(item);
            defaultOptions.Site = SiteContext.GetSite(siteInfo != null ? siteInfo.Name : "shell");

            var urlString = new UrlString(LinkManager.GetItemUrl(item, defaultOptions));
            urlString["sc_database"] = Client.ContentDatabase.Name;
            urlString["sc_duration"] = "temporary";
            urlString["sc_itemid"] = item.ID.ToString();
            urlString["sc_lang"] = item.Language.Name;
            urlString["sc_webedit"] = "0";
            return urlString.ToString();
        }

        private static string GetContent(string url, HttpCookieCollection cookies)
        {
            if (url.IndexOf("://", StringComparison.InvariantCulture) < 0)
            {
                url = WebUtil.GetServerUrl() + url;
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.CookieContainer = GetCookieContainer(url, cookies);

            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            var result = String.Empty;

            if (responseStream != null)
            {
                using (var sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }

            response.Close();
            return result;
        }

        private static CookieContainer GetCookieContainer(string url, HttpCookieCollection cookies)
        {
            var cookieName = ((SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState")).CookieName;
            var host = new Uri(url).Host;
            var cookieContainer = new CookieContainer();

            for (var index = 0; index < cookies.Count; ++index)
            {
                var httpCookie = cookies[index];
                if (httpCookie != null && cookieName != httpCookie.Name)
                {
                    cookieContainer.Add(new Cookie(httpCookie.Name, httpCookie.Value, httpCookie.Path, host));
                }
            }

            return cookieContainer;
        }
    }
}