using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class AccountBasedExpenseLineDetail
    {
        [JsonProperty("TaxCodeRef")]
        public Reference TaxCodeRef { get; set; }

        [JsonProperty("AccountRef")]
        public Reference AccountRef { get; set; }

        [JsonProperty("BillableStatus")]
        public string BillableStatus { get; set; }
    }
}
