using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using AwsTools;
using Newtonsoft.Json;
namespace FinanceApi.DatabaseModel
{
    public class FinanceUser : IModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        public static Dictionary<string, AttributeValue> GetKey(string email)
        {
            return new Dictionary<string, AttributeValue> { { "email", new AttributeValue { S = email } } };
        }

        public Dictionary<string, AttributeValue> GetKey()
        {
            return GetKey(Email);
        }

        public string GetTable()
        {
            return "Finance-Users";
        }
    }
}
