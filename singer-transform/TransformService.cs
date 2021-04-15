using HashidsNet;
using Octostache;
using SingerTransform.Models;
using SingerTransform.Octostache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SingerTransform
{
    public class TransformService
    {
        private Config _config;

        public TransformService(Config config)
        {
            _config = config;
        }

        public SingerOutput Transform(SingerOutput input)
        {
            var streamTransforms = _config.Transforms.Where(c => c.Stream == input.Stream).ToList();
            if (!streamTransforms.Any())
            {
                return input;
            }

            foreach (var transform in streamTransforms)
            {
                switch (transform.TransformType)
                {
                    case TransformType.RenameStream:
                        HandleRenameStream(input, transform);
                        break;
                    case TransformType.AddHashId:
                        HandleAddHashId(input, transform);
                        break;
                    case TransformType.CalculatedProperty:
                        HandleCalculatedField(input, transform);
                        break;
                    case TransformType.RenameProperty:
                        HandleRenameField(input, transform);
                        break;
                    case TransformType.FormatDate:
                        HandleFormatDate(input, transform);
                        break;
                }
            }

            return input;
        }

        private void HandleFormatDate(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (!input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return;

                input.Schema.Properties[transform.TransformProperty].Format = "date-time";
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                if (!input.Record.ContainsKey(transform.TransformProperty))
                    return;

                var strDate = input.Record[transform.TransformProperty].ToString();
                try
                {
                    var parsedDate = DateTime.ParseExact(strDate, "yyyyMMdd", null);
                    input.Record[transform.TransformProperty] = parsedDate.ToString("yyyy-MM-dd");
                }
                catch
                {
                }
            }
        }

        private void HandleAddHashId(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return;

                input.Schema.Properties.Add(transform.TransformProperty, new SingerSchemaProperty
                {
                    Type = transform.TransformPropertyType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformProperty);
                }
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                if (input.Record.ContainsKey(transform.TransformProperty))
                    return;

                var hashProps = transform.Settings ?? new Dictionary<string, string>();
                string salt = null;
                if (hashProps.ContainsKey("salt"))
                {
                    salt = hashProps["salt"];
                }
                int minHashLength = 0;
                if (hashProps.ContainsKey("minHashLength"))
                {
                    minHashLength = Convert.ToInt32(hashProps["minHashLength"]);
                }
                string alphabet = null;
                if (hashProps.ContainsKey("alphabet"))
                {
                    alphabet = hashProps["alphabet"];
                }
                string seps = null;
                if (hashProps.ContainsKey("seps"))
                {
                    seps = hashProps["seps"];
                }

                var hashId = new Hashids(salt, minHashLength, alphabet, seps);
                var valToHash = input.Record[transform.TransformValue].ToObject<int>();
                var hashedVal = hashId.Encode(valToHash);

                input.Record.Add(transform.TransformProperty, hashedVal);
            }
        }

        private void HandleRenameStream(SingerOutput input, TransformConfig transform)
        {
            // This transform applies to all input types

            input.Stream = transform.TransformValue;
        }

        private void HandleRenameField(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (!input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return;

                var propToRename = input.Schema.Properties[transform.TransformProperty];
                input.Schema.Properties.Remove(transform.TransformProperty);
                input.Schema.Properties.Add(transform.TransformValue, propToRename);
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                if (!input.Record.ContainsKey(transform.TransformProperty))
                    return;

                var propToRename = input.Record[transform.TransformProperty];
                input.Record.Remove(transform.TransformProperty);
                input.Record.Add(transform.TransformValue, propToRename);
            }
        }

        private void HandleCalculatedField(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return;

                input.Schema.Properties.Add(transform.TransformProperty, new SingerSchemaProperty
                {
                    Type = transform.TransformPropertyType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformProperty);
                }
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                var variables = new OctostacheDictionary();

                foreach (var prop in input.Record)
                {
                    variables.Add(prop.Key, prop.Value.ToString());
                }

                var calcVal = variables.Evaluate(transform.TransformValue);
                input.Record.Add(transform.TransformProperty, calcVal);
            }
        }
    }
}
