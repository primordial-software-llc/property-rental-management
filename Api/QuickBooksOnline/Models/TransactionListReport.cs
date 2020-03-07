using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.QuickBooksOnline
{
    public class TransactionListReport
    {
        [JsonProperty("Header")]
        public JObject Header { get; set; }

        [JsonProperty("Columns")]
        public JObject Columns { get; set; }

        [JsonProperty("Rows")]
        public JObject Rows { get; set; }
    }
}
