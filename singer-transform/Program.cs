using CommandLine;
using Newtonsoft.Json;
//using Serilog;
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
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Seq("http://host.docker.internal:5341")
            //    .Enrich.FromLogContext()
            //    .Enrich.WithProperty("AppName", "SingerTransform")
            //    .Enrich.WithProperty("InstanceId", Guid.NewGuid())
            //    .CreateLogger();

            Parser.Default
                .ParseArguments<Options>(args)
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
                            var singerOutput = SingerMessage.FromJson(s);

                            var transformed = transformService.Transform(singerOutput);

                            if (transformed != null)
                            {
                                var output = JsonConvert.SerializeObject(transformed);

                                //Log.Information("IN {inMessage} OUT {outMessage}", s, output);
                                Console.WriteLine(output);
                            }
							else
							{
                                //Log.Information("BLOCKED {message}", s);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Log.Error(ex, "TRANSFORM ERROR - {message}", s);
                            throw;
                        }
                    }
                });

        }
    }
}
