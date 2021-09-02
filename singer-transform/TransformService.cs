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

        public SingerMessage Transform(SingerMessage input)
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
                        input = HandleRenameStream(input, transform);
                        break;
                    case TransformType.AddHashId:
                        input = HandleAddHashId(input, transform);
                        break;
                    case TransformType.CalculatedProperty:
                        input = HandleCalculatedField(input, transform);
                        break;
                    case TransformType.RenameProperty:
                        input = HandleRenameField(input, transform);
                        break;
                    case TransformType.FormatDate:
                        input = HandleFormatDate(input, transform);
                        break;
                    case TransformType.NoTableVersioning:
                        input = HandleNoTableVersioning(input, transform);
                        break;
                }
            }

            return input;
        }

        private SingerMessage HandleFormatDate(SingerMessage input, TransformConfig transform)
        {
            if (input.Type == SingerMessageType.SCHEMA)
            {
                if (!input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return input;

                input.Schema.Properties[transform.TransformProperty].Format = "date-time";
            }
            else if (input.Type == SingerMessageType.RECORD)
            {
                if (!input.Record.ContainsKey(transform.TransformProperty))
                    return input;

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

            return input;
        }

        private SingerMessage HandleAddHashId(SingerMessage input, TransformConfig transform)
        {
            if (input.Type == SingerMessageType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return input;

                input.Schema.Properties.Add(transform.TransformProperty, new SingerSchemaProperty
                {
                    Type = transform.TransformPropertyType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformProperty);
                }
            }
            else if (input.Type == SingerMessageType.RECORD)
            {
                if (input.Record.ContainsKey(transform.TransformProperty))
                    return input;

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

            return input;
        }

        private SingerMessage HandleRenameStream(SingerMessage input, TransformConfig transform)
        {
            // This transform applies to all input types

            input.Stream = transform.TransformValue;

            return input;
        }

        private SingerMessage HandleRenameField(SingerMessage input, TransformConfig transform)
        {
            if (input.Type == SingerMessageType.SCHEMA)
            {
                if (!input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return input;

                var propToRename = input.Schema.Properties[transform.TransformProperty];
                input.Schema.Properties.Remove(transform.TransformProperty);
                input.Schema.Properties.Add(transform.TransformValue, propToRename);
            }
            else if (input.Type == SingerMessageType.RECORD)
            {
                if (!input.Record.ContainsKey(transform.TransformProperty))
                    return input;

                var propToRename = input.Record[transform.TransformProperty];
                input.Record.Remove(transform.TransformProperty);
                input.Record.Add(transform.TransformValue, propToRename);
            }

            return input;
        }

        private SingerMessage HandleCalculatedField(SingerMessage input, TransformConfig transform)
        {
            if (input.Type == SingerMessageType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformProperty))
                    return input;

                input.Schema.Properties.Add(transform.TransformProperty, new SingerSchemaProperty
                {
                    Type = transform.TransformPropertyType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformProperty);
                }
            }
            else if (input.Type == SingerMessageType.RECORD)
            {
                var variables = new OctostacheDictionary();

                foreach (var prop in input.Record)
                {
                    variables.Add(prop.Key, prop.Value.ToString());
                }

                var calcVal = variables.Evaluate(transform.TransformValue);
                input.Record.Add(transform.TransformProperty, calcVal);
            }

            return input;
        }

        private SingerMessage HandleNoTableVersioning(SingerMessage input, TransformConfig transform)
        {
            if (input.Type == SingerMessageType.ACTIVATE_VERSION)
            {
                // Don't pass on these messages
                return null;
            }
            if (input.Type == SingerMessageType.RECORD)
            {
                // Remove the versioning from the record messages
                input.Version = null;
            }

            return input;
        }
    }
}
