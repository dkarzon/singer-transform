using HashidsNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    public partial class TransformTests
    {
        private static Config ConfigFor_FormatDate()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "teststream",
                        TransformType = TransformType.FormatDate,
                        TransformProperty = "date"
                    }
                }
            };
        }

        [TestMethod]
        public void FormatDate_Schema()
        {
            var config = ConfigFor_FormatDate();

            var service = new TransformService(config);

            var usersSchemaInput = SingerMessage.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"teststream\", \"key_properties\": [\"date\"], \"schema\": {\"required\": [\"date\"], \"type\": [\"object\"], \"properties\": {\"date\": {\"type\": [\"string\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["date"]);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["date"].Format);
            Assert.AreEqual("date-time", usersSchemaOutput.Schema.Properties["date"].Format);
        }

        [TestMethod]
        public void FormatDate_Record()
        {
            var config = ConfigFor_FormatDate();

            var service = new TransformService(config);

            var usersRecordInput = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"date\": \"20210101\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("date"));
            Assert.AreEqual("2021-01-01", (string)usersRecordOutput.Record["date"]);
        }
    }
}
