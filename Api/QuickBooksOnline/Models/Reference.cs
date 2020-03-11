using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class Reference
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
