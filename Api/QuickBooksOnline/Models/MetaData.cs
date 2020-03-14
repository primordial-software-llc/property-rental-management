﻿using Newtonsoft.Json;

namespace Api.QuickBooksOnline.Models
{
    public class MetaData
    {
        [JsonProperty("CreateTime")]
        public string CreateTime { get; set; }

        [JsonProperty("LastUpdatedTime")]
        public string LastUpdatedTime { get; set; }
    }
}