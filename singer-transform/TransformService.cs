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
            if (input.Type == SingerOutputType.SCHEMA)
            {
                return HandleSchema(input);
            }

            if (input.Type == SingerOutputType.RECORD)
            {
                return HandleRecord(input);
            }

            //Unsupported type, do nothing
            return input;
        }

        private SingerOutput HandleSchema(SingerOutput input)
        {
            var streamTransforms = _config.Transforms.Where(c => c.Stream == input.Stream);
            if (!streamTransforms.Any())
            {
                return input;
            }

            foreach (var transform in streamTransforms)
            {
                switch (transform.TransformType)
                {
                    case TransformType.AddStaticField:
                        AddNewPropertyToSchema(input, transform);
                        break;
                }
            }

            return input;
        }

        private void AddNewPropertyToSchema(SingerOutput input, TransformConfig transform)
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

        private SingerOutput HandleRecord(SingerOutput input)
        {
            var streamTransforms = _config.Transforms.Where(c => c.Stream == input.Stream);
            if (!streamTransforms.Any())
            {
                return input;
            }

            foreach (var transform in streamTransforms)
            {
                switch (transform.TransformType)
                {
                    case TransformType.AddStaticField:
                        AddStaticPropertyToRecord(input, transform);
                        break;
                }
            }

            return input;
        }

        private void AddStaticPropertyToRecord(SingerOutput input, TransformConfig transform)
        {
            if (input.Record.ContainsKey(transform.TransformField))
                return;

            input.Record.Add(transform.TransformField, transform.TransformValue);
        }
    }
}
