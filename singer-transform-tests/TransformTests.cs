using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    [TestClass]
    public class TransformTests
    {
        private static Config ConfigFor_AddStaticField()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "users",
                        TransformType = TransformType.AddStaticField,
                        TransformValue = "TEST1",
                        TransformField = "new_field",
                        TransformFieldType = "string"
                    },
                    new TransformConfig
                    {
                        Stream = "users",
                        TransformType = TransformType.AddStaticField,
                        TransformValue = "TEST2",
                        TransformField = "new_key",
                        TransformFieldType = "string",
                        KeyProperty = true
                    }
                }
            };
        }

        [TestMethod]
        public void AddStaticField_Schema()
        {
            var config = ConfigFor_AddStaticField();

            var service = new TransformService(config);

            var usersSchemaInput = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"users\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            // Make sure the new field has been added to the output schema
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["new_field"]);
            Assert.AreEqual("string", usersSchemaOutput.Schema.Properties["new_field"].Type[0]);

            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["new_key"]);
            Assert.AreEqual("string", usersSchemaOutput.Schema.Properties["new_key"].Type[0]);
            Assert.IsTrue(usersSchemaOutput.KeyProperties.Contains("new_key"));
        }

        [TestMethod]
        public void AddStaticField_Record()
        {
            var config = ConfigFor_AddStaticField();

            var service = new TransformService(config);

            var usersRecordInput = SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"users\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            // Make sure the new field has been added to the output schema
            Assert.IsNotNull(usersRecordOutput.Record["new_field"]);
            Assert.AreEqual("TEST1", (string)usersRecordOutput.Record["new_field"]);

            Assert.IsNotNull(usersRecordOutput.Record["new_key"]);
            Assert.AreEqual("TEST2", (string)usersRecordOutput.Record["new_key"]);
        }
    }
}
