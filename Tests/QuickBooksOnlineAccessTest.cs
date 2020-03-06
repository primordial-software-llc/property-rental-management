using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Api;
using Api.QuickBooksOnline;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class QuickBooksOnlineAccessTest
    {
        private ITestOutputHelper Output { get; }

        public QuickBooksOnlineAccessTest(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Test_Request()
        {
            var accessToken = OAuthClient.GetAccessToken(
                Configuration.QuickBooksOnlineClientId,
                Configuration.QuickBooksOnlineClientSecret,
                Configuration.QuickBooksOnlineRefreshToken);

            var client = new HttpClient {BaseAddress = new Uri("https://quickbooks.api.intuit.com")};
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/v3/company/{Configuration.RealmId}/companyinfo/{Configuration.RealmId}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.SendAsync(request).Result;
            result.EnsureSuccessStatusCode();

            var response = result.Content.ReadAsStringAsync().Result;
            Output.WriteLine(response);
        }
    }
}
