using HashidsNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    public partial class TransformTests
    {
        private static Config ConfigFor_RenameStream()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "junkuserstablename",
                        TransformType = TransformType.RenameStream,
                        TransformValue = "users_table"
                    }
                }
            };
        }

        [TestMethod]
        public void RenameStream_Schema()
        {
            var config = ConfigFor_RenameStream();

            var service = new TransformService(config);

            var usersSchemaInput = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"junkuserstablename\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("users_table", usersSchemaOutput.Stream);

            var usersSchemaInput2 = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"notforrename\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput2 = service.Transform(usersSchemaInput2);

            Assert.IsNotNull(usersSchemaOutput2.Stream);
            Assert.AreEqual("notforrename", usersSchemaOutput2.Stream);
        }

        [TestMethod]
        public void RenameStream_Record()
        {
            var config = ConfigFor_RenameStream();

            var service = new TransformService(config);

            var usersRecordInput = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"junkuserstablename\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.AreEqual("users_table", usersRecordOutput.Stream);

            var usersRecordInput2 = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"notforrename\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput2 = service.Transform(usersRecordInput2);

            Assert.IsNotNull(usersRecordOutput2.Stream);
            Assert.AreEqual("notforrename", usersRecordOutput2.Stream);
        }
    }
}
