using HashidsNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    public partial class TransformTests
    {
        private static Config ConfigFor_RenameField()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "teststream",
                        TransformType = TransformType.RenameProperty,
                        TransformValue = "newid",
                        TransformProperty = "id"
                    }
                }
            };
        }

        [TestMethod]
        public void RenameField_Schema()
        {
            var config = ConfigFor_RenameField();

            var service = new TransformService(config);

            var usersSchemaInput = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"teststream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("teststream", usersSchemaOutput.Stream);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["newid"]);
        }

        [TestMethod]
        public void RenameField_Record()
        {
            var config = ConfigFor_RenameField();

            var service = new TransformService(config);

            var usersRecordInput = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"id\": 1}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("newid"));
            Assert.AreEqual("1", (string)usersRecordOutput.Record["newid"]);
        }
    }
}
