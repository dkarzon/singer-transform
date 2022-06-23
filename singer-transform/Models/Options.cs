using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public class Options
    {
        [Option('c', "config", Required = true, HelpText = "A required argument that points to a JSON file containing the transformation configuration.")]
        public string Config { get; set; }
    }
}
