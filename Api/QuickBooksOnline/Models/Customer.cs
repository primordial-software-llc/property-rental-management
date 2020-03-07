using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class Customer : IQuickBooksOnlineEntity
    {
        [JsonIgnore]
        public string EntityName => "Customer";

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
    }
}
