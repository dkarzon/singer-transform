using HashidsNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    public partial class TransformTests
    {
        private static Config ConfigFor_AddHashId()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "teststream",
                        TransformType = TransformType.AddHashId,
                        TransformValue = "id",
                        TransformProperty = "hashid",
                        TransformPropertyType = new List<string> { "string" },
                        Settings = new Dictionary<string, string>
                        {
                            { "salt", "salt" },
                            { "minHashLength", "5" },
                            { "alphabet", "abcdefghijklmnopqrstuvwxyz1234567890" },
                            { "seps", "cfhistu" }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void AddHashId_Schema()
        {
            var config = ConfigFor_AddHashId();

            var service = new TransformService(config);

            var usersSchemaInput = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"teststream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("teststream", usersSchemaOutput.Stream);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["hashid"]);

            var usersSchemaInput2 = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"otherstream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput2 = service.Transform(usersSchemaInput2);

            Assert.IsNotNull(usersSchemaOutput2.Stream);
            Assert.IsFalse(usersSchemaOutput2.Schema.Properties.ContainsKey("hashid"));
        }

        [TestMethod]
        public void AddHashId_Record()
        {
            var config = ConfigFor_AddHashId();

            var service = new TransformService(config);

            var usersRecordInput = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("hashid"));
        }
    }
}
