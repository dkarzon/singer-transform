using CommandLine;
using Newtonsoft.Json;
using SingerTransform.Models;
using System;

namespace SingerTransform
{
    class Program
    {
        public class Options
        {
            [Option('c', "config", Required = true, HelpText = "A required argument that points to a JSON file containing the transformation configuration.")]
            public string Config { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       var configJson = System.IO.File.ReadAllText(o.Config);
                       
                       var config = JsonConvert.DeserializeObject<Config>(configJson);

                       var transformService = new TransformService(config);

                       string s;
                       while ((s = Console.ReadLine()) != null)
                       {
                           try
                           {
                               var singerOutput = SingerOutput.FromJson(s);

                               var transformed = transformService.Transform(singerOutput);

                               Console.WriteLine(JsonConvert.SerializeObject(transformed));
                           }
                           catch (Exception ex)
                           {
                               throw;
                           }
                       }
                   });

        }
    }
}
