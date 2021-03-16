using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SingerTransform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class SingerSchema
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Type { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, SingerSchemaProperty> Properties { get; set; }

        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Required { get; set; }
    }
}
