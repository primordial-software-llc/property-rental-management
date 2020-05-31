using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyRentalManagement.DatabaseModel;
using PropertyRentalManagement.QuickBooksOnline.Models;

namespace PropertyRentalManagement.QuickBooksOnline
{
    public class QuickBooksOnlineClient
    {
        public string AccessToken { get; set; }
        public ILogger Logger { get; }

        public QuickBooksOnlineClient(QuickBooksOnlineConnection connection, ILogger logger)
        {
            Logger = logger;
            AccessToken = OAuthClient.GetAccessToken(
                connection.ClientId,
                connection.ClientSecret,
                connection.RefreshToken,
                logger);
        }

        public int QueryCount<T>(string query) where T : IQuickBooksOnlineEntity, new()
        {
            var result = Request($"query?query={HttpUtility.UrlEncode(query)}", HttpMethod.Get);
            var json = JObject.Parse(result);
            return json["QueryResponse"]["totalCount"].Value<int>();
        }

        public IList<T> QueryAll<T>(string query) where T : IQuickBooksOnlineEntity, new()
        {
            var count = QueryCount<T>(query.Replace("select * from", "select count(*)from"));
            var maxResults = 100;
            var allResults = new List<T>();
            for (int startPosition = 0; startPosition < count; startPosition += maxResults)
            {
                var pagedQuery = $"{query} STARTPOSITION {startPosition} MAXRESULTS {maxResults}";
                var result = Request($"query?query={HttpUtility.UrlEncode(pagedQuery)}", HttpMethod.Get);
                var json = JObject.Parse(result);
                var entityResults = json["QueryResponse"][new T().EntityName];
                allResults.AddRange(JsonConvert.DeserializeObject<IList<T>>(entityResults.ToString()));
            }

            return allResults;
        }

        public IList<T> Query<T>(string query) where T : IQuickBooksOnlineEntity, new()
        {
            var result = Request($"query?query={HttpUtility.UrlEncode(query)}", HttpMethod.Get);
            var json = JObject.Parse(result);
            var entityResults = json["QueryResponse"][new T().EntityName];
            if (entityResults == null)
            {
                return new List<T>();
            }
            return JsonConvert.DeserializeObject<IList<T>>(entityResults.ToString());
        }

        public string Request(string path, HttpMethod method, string body = null)
        {
            var client = new HttpClient { BaseAddress = new Uri("https://quickbooks.api.intuit.com") };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var relativePath = $"/v3/company/{Configuration.RealmId}/{path}";
            var request = new HttpRequestMessage(method, relativePath);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var result = client.SendAsync(request).Result;
            var response = result.Content.ReadAsStringAsync().Result;
            if (!result.IsSuccessStatusCode)
            {
                Logger.Log($"QuickBooks Online API Request Failure"
                           + $" {(int) result.StatusCode} GET {relativePath}"
                           + $" Received {response}");
            }
            result.EnsureSuccessStatusCode();

            return response;
        }
    }
}
