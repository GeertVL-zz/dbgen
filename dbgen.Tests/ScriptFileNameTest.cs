using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dbgen.Tests
{
    [TestClass]
    public class ScriptFileNameTest
    {
        [TestMethod]
        public void ScriptFileName_Empty()
        {
            string fileName = string.Empty;

            var actual = new ScriptFileName(fileName);

            Assert.AreEqual(null, actual.ScriptType);
            Assert.AreEqual(null, actual.TimeStamp);
        }

        [TestMethod]
        public void ScriptFileName_Null()
        {
            string fileName = string.Empty;

            var actual = new ScriptFileName(fileName);

            Assert.AreEqual(null, actual.ScriptType);
            Assert.AreEqual(null, actual.TimeStamp);
        }

        [TestMethod]
        public void ScriptFileName_Default()
        {
            string fileName = "db_data_20111212121212.sql";

            var actual = new ScriptFileName(fileName);

            Assert.AreEqual("data", actual.ScriptType);
            Assert.AreEqual("20111212121212", actual.TimeStamp);
        }
    }
}
