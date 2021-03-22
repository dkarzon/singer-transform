using Newtonsoft.Json;
using SingerTransform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class TransformConfig
    {
        [JsonProperty("stream")]
        public string Stream { get; set; }

        [JsonProperty("transformType")]
        public TransformType TransformType { get; set; }

        [JsonProperty("value")]
        public string TransformValue { get; set; }

        [JsonProperty("field")]
        public string TransformField { get; set; }

        [JsonProperty("fieldType")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> TransformFieldType { get; set; }

        [JsonProperty("keyProperty")]
        public bool KeyProperty { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
