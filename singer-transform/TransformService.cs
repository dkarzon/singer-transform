using HashidsNet;
using Octostache;
using SingerTransform.Models;
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
                    case TransformType.CalculatedField:
                        HandleCalculatedField(input, transform);
                        break;
                    case TransformType.RenameField:
                        HandleRenameField(input, transform);
                        break;
                }
            }

            return input;
        }

        private void HandleAddHashId(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformField))
                    return;

                input.Schema.Properties.Add(transform.TransformField, new SingerSchemaProperty
                {
                    Type = transform.TransformFieldType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformField);
                }
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                if (input.Record.ContainsKey(transform.TransformField))
                    return;

                var hashProps = transform.Properties ?? new Dictionary<string, string>();
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

                input.Record.Add(transform.TransformField, hashedVal);
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
                if (!input.Schema.Properties.ContainsKey(transform.TransformField))
                    return;

                var propToRename = input.Schema.Properties[transform.TransformField];
                input.Schema.Properties.Remove(transform.TransformField);
                input.Schema.Properties.Add(transform.TransformValue, propToRename);
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                if (!input.Record.ContainsKey(transform.TransformField))
                    return;

                var propToRename = input.Record[transform.TransformField];
                input.Record.Remove(transform.TransformField);
                input.Record.Add(transform.TransformValue, propToRename);
            }
        }

        private void HandleCalculatedField(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformField))
                    return;

                input.Schema.Properties.Add(transform.TransformField, new SingerSchemaProperty
                {
                    Type = transform.TransformFieldType
                });

                if (transform.KeyProperty)
                {
                    input.KeyProperties.Add(transform.TransformField);
                }
            }
            else if (input.Type == SingerOutputType.RECORD)
            {
                var variables = new VariableDictionary();

                foreach (var prop in input.Record)
                {
                    variables.Add(prop.Key, prop.Value.ToString());
                }

                var calcVal = variables.Evaluate(transform.TransformValue);
                input.Record.Add(transform.TransformField, calcVal);
            }
        }
    }
}
