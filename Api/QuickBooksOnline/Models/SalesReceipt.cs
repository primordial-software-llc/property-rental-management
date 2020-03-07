using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class SalesReceipt : IQuickBooksOnlineEntity
    {
        [JsonIgnore]
        public string EntityName => "SalesReceipt";

        [JsonProperty("TxnDate")]
        public string TxnDate { get; set; }
    }
}
