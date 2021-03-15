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
    public class ConfigTests
    {
        [TestMethod]
        public void ConfigParseTest()
        {
            var configJson = "{\"transforms\": [{\"stream\": \"ga_acquisition\",\"transformType\": \"AddStaticField\",\"value\": \"##SITE##\",\"field\": \"ga_site\",\"fieldType\": \"string\"}]}";

            var config = JsonConvert.DeserializeObject<Config>(configJson);

            Assert.AreEqual(TransformType.AddStaticField, config.Transforms[0].TransformType);
        }
    }
}
