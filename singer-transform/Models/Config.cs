using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class Config
    {
        [JsonProperty("transforms")]
        public List<TransformConfig> Transforms { get; set; } = new List<TransformConfig>();
    }
}
