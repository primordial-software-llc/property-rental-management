﻿using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using AwsTools;
using Newtonsoft.Json;

namespace Api.DatabaseModel
{
    public class Vendor : IModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("quickBooksOnlineId")]
        public int QuickBooksOnlineId { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("paymentFrequency")]
        public string PaymentFrequency { get; set; }

        public static Dictionary<string, AttributeValue> GetKey(string email)
        {
            return new Dictionary<string, AttributeValue> { { "id", new AttributeValue { S = email } } };
        }

        public Dictionary<string, AttributeValue> GetKey()
        {
            return GetKey(Id);
        }

        public string GetTable()
        {
            return "lakeland-mi-pueblo-vendors";
        }
    }
}