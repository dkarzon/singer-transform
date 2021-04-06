using HashidsNet;
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
                        TransformType = TransformType.CalculatedField,
                        TransformValue = "TEST1",
                        TransformField = "new_field",
                        TransformFieldType = new List<string> { "string" }
                    },
                    new TransformConfig
                    {
                        Stream = "users",
                        TransformType = TransformType.CalculatedField,
                        TransformValue = "TEST2",
                        TransformField = "new_key",
                        TransformFieldType = new List<string> { "string" },
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

            // Make sure the new field has been added to the output record
            Assert.IsNotNull(usersRecordOutput.Record["new_field"]);
            Assert.AreEqual("TEST1", (string)usersRecordOutput.Record["new_field"]);

            Assert.IsNotNull(usersRecordOutput.Record["new_key"]);
            Assert.AreEqual("TEST2", (string)usersRecordOutput.Record["new_key"]);
        }


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

            var usersSchemaInput = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"junkuserstablename\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            // Make sure the output stream has the new name
            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("users_table", usersSchemaOutput.Stream);

            var usersSchemaInput2 = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"notforrename\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput2 = service.Transform(usersSchemaInput2);

            // Make sure the output stream has the old name
            Assert.IsNotNull(usersSchemaOutput2.Stream);
            Assert.AreEqual("notforrename", usersSchemaOutput2.Stream);
        }

        [TestMethod]
        public void RenameStream_Record()
        {
            var config = ConfigFor_RenameStream();

            var service = new TransformService(config);

            var usersRecordInput = SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"junkuserstablename\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            // Make sure the output stream has the new name
            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.AreEqual("users_table", usersRecordOutput.Stream);

            var usersRecordInput2 = SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"notforrename\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput2 = service.Transform(usersRecordInput2);

            // Make sure the output stream has the old name
            Assert.IsNotNull(usersRecordOutput2.Stream);
            Assert.AreEqual("notforrename", usersRecordOutput2.Stream);
        }


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
                        TransformField = "hashid",
                        TransformFieldType = new List<string> { "string" },
                        Properties = new Dictionary<string, string>
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

            var usersSchemaInput = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"teststream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            // Check new field added
            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("teststream", usersSchemaOutput.Stream);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["hashid"]);

            var usersSchemaInput2 = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"otherstream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput2 = service.Transform(usersSchemaInput2);

            // Make sure the output stream has the old name
            Assert.IsNotNull(usersSchemaOutput2.Stream);
            Assert.IsFalse(usersSchemaOutput2.Schema.Properties.ContainsKey("hashid"));
        }

        [TestMethod]
        public void AddHashId_Record()
        {
            var config = ConfigFor_AddHashId();

            var service = new TransformService(config);

            var usersRecordInput = SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"id\": 1, \"name\": \"Chris\"}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            // Make sure the output stream has the new name
            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("hashid"));
        }

        private static Config ConfigFor_CalculatedField()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "teststream",
                        TransformType = TransformType.CalculatedField,
                        TransformValue = "#{id}-new",
                        TransformField = "newid",
                        TransformFieldType = new List<string> { "string" }
                    }
                }
            };
        }

        [TestMethod]
        public void CalculatedField_Schema()
        {
            var config = ConfigFor_CalculatedField();

            var service = new TransformService(config);

            var usersSchemaInput = SingerOutput.FromJson("{\"type\": \"SCHEMA\", \"stream\": \"teststream\", \"key_properties\": [\"id\"], \"schema\": {\"required\": [\"id\"], \"type\": [\"object\"], \"properties\": {\"id\": {\"type\": [\"integer\"]}}}}");
            var usersSchemaOutput = service.Transform(usersSchemaInput);

            // Check new field added
            Assert.IsNotNull(usersSchemaOutput.Stream);
            Assert.AreEqual("teststream", usersSchemaOutput.Stream);
            Assert.IsNotNull(usersSchemaOutput.Schema.Properties["newid"]);
        }

        [TestMethod]
        public void CalculatedField_Record()
        {
            var config = ConfigFor_CalculatedField();

            var service = new TransformService(config);

            var usersRecordInput = SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"id\": 1}}");
            var usersRecordOutput = service.Transform(usersRecordInput);

            // Make sure the output stream has the new name
            Assert.IsNotNull(usersRecordOutput.Stream);
            Assert.IsTrue(usersRecordOutput.Record.ContainsKey("newid"));
            Assert.AreEqual("1-new", (string)usersRecordOutput.Record["newid"]);
        }

        private static Config ConfigFor_CalculatedField2()
        {
            return new Config
            {
                Transforms = new List<TransformConfig>
                {
                    new TransformConfig
                    {
                        Stream = "teststream",
                        TransformType = TransformType.CalculatedField,
                        TransformValue = "#{url|cleanurl}",
                        TransformField = "cleanurl",
                        TransformFieldType = new List<string> { "string" }
                    }
                }
            };
        }

        [TestMethod]
        public void CalculatedField2_Record()
        {
            var config = ConfigFor_CalculatedField2();

            var service = new TransformService(config);

            var usersRecordOutput = service.Transform(SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"url\": \"/\"}}"));

            Assert.AreEqual("/", (string)usersRecordOutput.Record["cleanurl"]);

            usersRecordOutput = service.Transform(SingerOutput.FromJson("{\"type\": \"RECORD\", \"stream\": \"teststream\", \"record\": {\"url\": \"/testurl?q=search\"}}"));

            Assert.AreEqual("/testurl", (string)usersRecordOutput.Record["cleanurl"]);
        }
    }
}
