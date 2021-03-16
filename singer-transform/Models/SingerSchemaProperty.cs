using Newtonsoft.Json;
using SingerTransform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class SingerSchemaProperty
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Type { get; set; }

        [JsonProperty("format", NullValueHandling=NullValueHandling.Ignore)]
        public string Format { get; set; }
    }
}
