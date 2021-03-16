using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class SingerOutput
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("time_extracted", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeExtracted { get; set; }

        [JsonProperty("record", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Record { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Value { get; set; }

        [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
        public SingerSchema Schema { get; set; }

        [JsonProperty("key_properties", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> KeyProperties { get; set; }

        [JsonProperty("bookmark_properties", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> BookmarkProperties { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> Children { get; set; }

        public static SingerOutput FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SingerOutput>(json);
        }
    }
}
