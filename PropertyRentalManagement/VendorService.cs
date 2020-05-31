using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyRentalManagement.DatabaseModel;

namespace Api
{
    public class VendorService
    {
        public void Create(IAmazonDynamoDB dbClient, int quickBooksOnlineId, bool isActive, string paymentFrequency)
        {
            var user = new Vendor
            {
                Id = Guid.NewGuid().ToString(),
                QuickBooksOnlineId = quickBooksOnlineId,
                IsActive = isActive,
                PaymentFrequency = paymentFrequency
            };
            var update = JObject.FromObject(user, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
            dbClient.PutItemAsync(
                new Vendor().GetTable(),
                Document.FromJson(update.ToString()).ToAttributeMap()
            ).Wait();
        }
    }
}
