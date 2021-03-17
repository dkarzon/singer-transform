using HashidsNet;
using SingerTransform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    case TransformType.AddStaticField:
                        HandleAddStaticField(input, transform);
                        break;
                    case TransformType.RenameStream:
                        HandleRenameStream(input, transform);
                        break;
                    case TransformType.AddHashId:
                        HandleAddHashId(input, transform);
                        break;
                }
            }

            return input;
        }

        private void HandleAddStaticField(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformField))
                    return;

                input.Schema.Properties.Add(transform.TransformField, new SingerSchemaProperty
                {
                    Type = new List<string>
                    {
                        transform.TransformFieldType
                    }
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

                input.Record.Add(transform.TransformField, transform.TransformValue);
            }
        }

        private void HandleAddHashId(SingerOutput input, TransformConfig transform)
        {
            if (input.Type == SingerOutputType.SCHEMA)
            {
                if (input.Schema.Properties.ContainsKey(transform.TransformField))
                    return;

                input.Schema.Properties.Add(transform.TransformField, new SingerSchemaProperty
                {
                    Type = new List<string>
                    {
                        transform.TransformFieldType ?? "string"
                    }
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
    }
}
