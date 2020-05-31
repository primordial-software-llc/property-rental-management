using Newtonsoft.Json;

namespace PropertyRentalManagement.QuickBooksOnline.Models
{
    public class Reference
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
