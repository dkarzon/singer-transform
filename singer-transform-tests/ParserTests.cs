using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SingerTransform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ConfigParseEnumTest()
        {
            var configJson = "{\"transforms\": [{\"stream\": \"ga_acquisition\",\"transformType\": \"CalculatedProperty\",\"value\": \"##SITE##\",\"property\": \"ga_site\",\"propertyType\": \"string\"}]}";

            var config = JsonConvert.DeserializeObject<Config>(configJson);

            Assert.AreEqual(TransformType.CalculatedProperty, config.Transforms[0].TransformType);
        }

        [TestMethod]
        public void SchemaParseSingleOrArrayTest()
        {
            var outputJsonSingle = "{\"schema\": {\"type\": \"object\"}}";
            var outputJsonArray = "{\"schema\": {\"type\": [\"string\",\"null\"]}}";

            var outputSingle = SingerMessage.FromJson(outputJsonSingle);
            var outputArray = SingerMessage.FromJson(outputJsonArray);

            Assert.AreEqual(1, outputSingle.Schema.Type.Count);
            Assert.AreEqual("object", outputSingle.Schema.Type[0]);

            Assert.AreEqual(2, outputArray.Schema.Type.Count);
            Assert.AreEqual("string", outputArray.Schema.Type[0]);
        }
    }
}
