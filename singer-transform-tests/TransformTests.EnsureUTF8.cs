using HashidsNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingerTransform;
using SingerTransform.Models;
using System.Collections.Generic;

namespace SingerTransform.Tests
{
    public partial class TransformTests
    {
        private static Config ConfigFor_EnsureUTF8()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "table1",
                        TransformType = TransformType.EnsureUTF8,
                        TransformProperty = "name"
                    }
                }
            };
        }

        [TestMethod]
        public void EnsureUTF8_Record()
        {
            var config = ConfigFor_EnsureUTF8();

            var service = new TransformService(config);

            var usersRecordInput = SingerMessage.FromJson("{\"type\": \"RECORD\", \"stream\": \"table1\", \"record\": {\"id\": 1, \"name\": \"te\x00\x00\x00ster\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("name"));
            Assert.AreEqual("tester", (string)usersRecordOutput.Record["name"]);
        }
    }
}
