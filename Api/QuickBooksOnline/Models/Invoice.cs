using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class Invoice : IQuickBooksOnlineEntity
    {
        [JsonIgnore]
        public string EntityName => "Invoice";

        [JsonProperty("TxnDate")]
        public string TxnDate { get; set; }
    }
}
