﻿using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AwsTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api
{
    public class DatabaseClient<T> where T : IModel, new()
    {
        private IAmazonDynamoDB Client { get; }

        public DatabaseClient(IAmazonDynamoDB client)
        {
            Client = client;
        }

        public T Get(T model)
        {
            var dbItem = Client.GetItemAsync(model.GetTable(), model.GetKey()).Result;
            return JsonConvert.DeserializeObject<T>(Document.FromAttributeMap(dbItem.Item).ToJson());
        }

        public void Update(Dictionary<string, AttributeValue> key, T model)
        {
            var update = JObject.FromObject(model, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
            var updates = Document.FromJson(update.ToString()).ToAttributeUpdateMap(false);
            Client.UpdateItemAsync(
                new T().GetTable(),
                key,
                updates
            ).Wait();
        }
    }
}