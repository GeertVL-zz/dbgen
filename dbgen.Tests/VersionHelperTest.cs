using dbgen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace dbgen.Tests
{
    [TestClass()]
    public class VersionHelperTest
    {
        [TestMethod()]
        public void ConvertTimeStamp_DefaultValue()
        {
            string timestamp = "20111122121212"; 
            DateTime expected = new DateTime(2011, 11, 22, 12, 12, 12);
            
            DateTime actual = VersionHelper.ConvertTimeStamp(timestamp);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ConvertTimeStamp_Empty()
        {
            string timestamp = string.Empty;
            DateTime expected = DateTime.MinValue;

            DateTime actual = VersionHelper.ConvertTimeStamp(timestamp);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ConvertTimeStamp_Null()
        {
            string timestamp = null;
            DateTime expected = DateTime.MinValue;

            DateTime actual = VersionHelper.ConvertTimeStamp(timestamp);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ConvertTimeStamp_IncorrectLength()
        {
            string timestamp = "47474674545436356";
            DateTime expected = DateTime.MinValue;

            DateTime actual = VersionHelper.ConvertTimeStamp(timestamp);

            Assert.AreEqual(expected, actual);
        }
    }
}
