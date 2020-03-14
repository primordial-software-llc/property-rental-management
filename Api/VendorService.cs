﻿using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Api.DatabaseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api
{
    public class VendorService
    {
        public void Create(int quickBooksOnlineId, bool isActive, string paymentFrequency)
        {
            var user = new Vendor
            {
                Id = Guid.NewGuid().ToString(),
                QuickBooksOnlineId = quickBooksOnlineId,
                IsActive = isActive,
                PaymentFrequency = paymentFrequency
            };
            var update = JObject.FromObject(user, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
            var dbClient = new AmazonDynamoDBClient();
            dbClient.PutItemAsync(
                new Vendor().GetTable(),
                Document.FromJson(update.ToString()).ToAttributeMap()
            ).Wait();
        }
    }
}