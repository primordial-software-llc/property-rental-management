using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class Payment : IQuickBooksOnlineEntity
    {
        [JsonIgnore]
        public string EntityName => "Payment";

        [JsonProperty("TxnDate")]
        public string TxnDate { get; set; }
    }
}
