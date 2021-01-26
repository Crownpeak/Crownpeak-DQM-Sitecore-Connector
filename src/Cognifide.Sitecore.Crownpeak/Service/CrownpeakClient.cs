using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Crownpeak.Sitecore.DQM.DAO;
using Crownpeak.Sitecore.DQM.Logic;
using Crownpeak.Sitecore.DQM.Service.Model;
using Newtonsoft.Json;

namespace Crownpeak.Sitecore.DQM.Service
{
    public class CrownpeakClient
    {
        private readonly Uri _apiBaseUrl;

        private readonly string _apiKey;

        private readonly int _retryCount;

        public CrownpeakClient(string apiKey, int retryCount)
        {
            if (Settings.ApiUrl != "")
            {
                _apiBaseUrl = new Uri(Settings.ApiUrl);
            }
            else
            {
                _apiBaseUrl = new Uri("https://api.crownpeak.net/dqm-cms/v1");
            }

            _apiKey = apiKey;
            _retryCount = retryCount;
        }

        public Website[] GetWebsites()
        {
            var json = Execute("GET", "/websites");
            return JsonConvert.DeserializeObject<Website[]>(json ?? string.Empty);
        }

        public Website GetWebsite(string websiteId)
        {
            var json = Execute("GET", String.Format("/websites/{0}", websiteId));
            return JsonConvert.DeserializeObject<Website>(json ?? string.Empty);
        }

        public Checkpoint[] GetWebsiteCheckpoints(string websiteId)
        {
            var json = Execute("GET", string.Format("/websites/{0}/checkpoints", websiteId));

            DAOErrorLog.GetInstance.ErrorLogWrite("GetWebsiteCheckpoints", "Request", "WebsiteId : " + websiteId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetWebsiteCheckpoints", "Response", json);

            return JsonConvert.DeserializeObject<Checkpoint[]>(json ?? string.Empty);
        }

        public Checkpoint[] GetCheckpoints()
        {
            var json = Execute("GET", "/checkpoints");

            DAOErrorLog.GetInstance.ErrorLogWrite("GetCheckpoints", "Request", "Checkpoints");
            DAOErrorLog.GetInstance.ErrorLogWrite("GetCheckpoints", "Response", json);

            return JsonConvert.DeserializeObject<Checkpoint[]>(json ?? string.Empty);
        }

        public string HightlightAll(string assetId)
        {
            var data = new NameValueCollection
            {
                {"visibility", "public"},
            };

            var json = Execute("GET", "/assets/{0}/pagehighlight/all", data);

            DAOErrorLog.GetInstance.ErrorLogWrite("HightlightAll", "Request", "AssetId : " + assetId + ", Data : 'visibility', 'public'");
            DAOErrorLog.GetInstance.ErrorLogWrite("HightlightAll", "Response", json);

            return json;
        }

        public Checkpoint GetCheckpoint(string checkpointId)
        {
            var json = Execute("GET", string.Format("/checkpoints/{0}", checkpointId));

            DAOErrorLog.GetInstance.ErrorLogWrite("GetCheckpoint", "Request", "CheckpointId : " + checkpointId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetCheckpoint", "Response", json);

            return JsonConvert.DeserializeObject<Checkpoint>(json ?? string.Empty);
        }

        public Asset CreateAsset(string websiteId, string content, string contentType, string url = null)
        {
            var data = new NameValueCollection
                           {
                               {"websiteId", websiteId},
                               {"content", content},
                               {"contentType", contentType},
                           };
            if (url != null)
            {
                data.Add("url", url);
            }

            var json = Execute("POST", "/assets", null, data);

            DAOErrorLog.GetInstance.ErrorLogWrite("CreateAsset", "Request", "WebsiteId : " + websiteId + ", ContentType : " + contentType + ", Url : " + url + ", Content : " + content);
            DAOErrorLog.GetInstance.ErrorLogWrite("CreateAsset", "Response", json);

            return JsonConvert.DeserializeObject<Asset>(json ?? string.Empty);
        }

        public Asset UpdateAsset(string assetId, string content)
        {
            

            var data = new NameValueCollection
                           {
                               {"content", content},
                           };

            var json = Execute("PUT", string.Format("/assets/{0}", assetId), null, data);

            DAOErrorLog.GetInstance.ErrorLogWrite("UpdateAsset", "Request", "AssetId : " + assetId + ", Content : " + content);
            DAOErrorLog.GetInstance.ErrorLogWrite("UpdateAsset", "Respose", json);

            return JsonConvert.DeserializeObject<Asset>(json ?? string.Empty);
        }

        public bool DeleteAsset(string assetId)
        {
            return Execute("DELETE", string.Format("/assets/{0}", assetId)) != null;
        }

        public Asset GetAsset(string assetId)
        {
            var json = Execute("GET", string.Format("/assets/{0}", assetId));

            DAOErrorLog.GetInstance.ErrorLogWrite("GetAsset", "Request", "AssetId : " + assetId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetAsset", "Respose", json);

            return JsonConvert.DeserializeObject<Asset>(json ?? string.Empty);
        }

        public string GetAssetContent(string assetId)
        {
            var str = Execute("GET", string.Format("/assets/{0}/content", assetId));

            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetContent", "Request", "AssetId : " + assetId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetContent", "Response", str);

            return str;
        }

        public AssetStatus GetAssetStatus(string assetId)
        {
            var json = Execute("GET", string.Format("/assets/{0}/status", assetId));

            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetStatus", "Request", "AssetId : " + assetId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetStatus", "Response", json);

            return JsonConvert.DeserializeObject<AssetStatus>(json ?? string.Empty);
        }

        public string GetAssetError(string assetId, string checkpointId, bool highlightSource = false)
        {
            var query = new NameValueCollection
                            {
                                {"highlightSource", highlightSource.ToString()},
                            };

            var str = Execute("GET", string.Format("/assets/{0}/errors/{1}", assetId, checkpointId), query);

            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetError", "Request", "AssetId : " + assetId + " CheckpointId : " + checkpointId + " HighlightSource : " + highlightSource.ToString());
            DAOErrorLog.GetInstance.ErrorLogWrite("GetAssetError", "Response", str);

            return str;
        }

        public string GetPageHighlight(string assetId)
        {
            var query = new NameValueCollection
            {
                {"color", "ff0000"},
            };

            var str = Execute("GET", string.Format("/assets/{0}/pagehighlight/all", assetId), accept: "text/html; charset=UTF-8",
                query: query);

            DAOErrorLog.GetInstance.ErrorLogWrite("GetPageHighlight", "Request", "AssetId : " + assetId);
            DAOErrorLog.GetInstance.ErrorLogWrite("GetPageHighlight", "Response", str);

            return str;
        }

        public SearchResult FindAssets(string websiteId, string url, int limit = 20)
        {
            var query = new NameValueCollection
                            {
                                {"limit", limit.ToString()},
                            };
            if (websiteId != null)
            {
                query.Add("websiteId", websiteId);
            }
            if (url != null)
            {
                query.Add("url", url);
            }

            var json = Execute("GET", "/assets", query);
            return JsonConvert.DeserializeObject<SearchResult>(json ?? string.Empty);
        }

        private string Execute(string method, string url, NameValueCollection query = null, NameValueCollection data = null,
            string accept = "application/json; charset=UTF-8")
        {
            var uriBuilder = GetUri(url, query);

            for (var i = 0; i < _retryCount; i++)
            {
                using (var client = GetClient(accept))
                {
                    try
                    {
                        if (method == "GET")
                        {
                            return client.DownloadString(uriBuilder);
                        }

                        if (method == "DELETE")
                        {
                            return client.UploadString(uriBuilder, method, string.Empty);
                        }

                        if (data != null)
                        {
                            var dataString = HttpUtility.ParseQueryString("");
                            dataString.Add(data);

                            return Encoding.UTF8.GetString(client.UploadValues(uriBuilder, method, dataString));
                        }
                    }
                    catch (WebException ex)
                    {
                        new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

                        if (WaitOnRetryAfterHeader(ex))
                        {
                            continue;
                        }

                        throw new WebException("Crownpeak call " + uriBuilder + ": " + ex.Message, ex.Status);
                    }
                }
            }
            return string.Empty;
        }

        private WebClient GetClient(string accept)
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };
            webClient.Headers.Add("x-api-key", _apiKey);
            webClient.Headers.Add("Accept", accept);
            return webClient;
        }

        private Uri GetUri(string url, NameValueCollection query)
        {
            var queryString = HttpUtility.ParseQueryString("");
            if (query != null)
            {
                queryString.Add(query);
            }
            queryString.Add("apiKey", _apiKey);

            var uriBuilder = new UriBuilder(_apiBaseUrl);
            uriBuilder.Path = uriBuilder.Path + url;
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.Uri;
        }

        private static bool WaitOnRetryAfterHeader(WebException exception)
        {
            var retryAfterHeader = exception.Response?.Headers?["Retry-After"];
            if (retryAfterHeader != null)
            {
                int retryAfter;
                if (int.TryParse(retryAfterHeader, out retryAfter) && retryAfter > 0)
                {
                    Thread.Sleep(new TimeSpan(0, 0, retryAfter));
                    return true;
                }
            }
            return false;
        }
    }
}
