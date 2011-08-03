using dbgen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace dbgen.Tests
{
    [TestClass()]
    public class ScriptFileHelperTest
    {
        [TestMethod()]
        public void GetScriptFileTimeStamp_Empty()
        {
            string scriptFileName = string.Empty; 
            string expected = null; 
            string actual;
            actual = ScriptFileHelper.GetScriptFileTimeStamp(scriptFileName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptFileTimeStamp_Null()
        {
            string scriptFileName = null;
            string expected = null;
            string actual;
            actual = ScriptFileHelper.GetScriptFileTimeStamp(scriptFileName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptFileTimeStamp_Default()
        {
            string scriptFileName = "db_general_20111212121212.sql";
            string expected = "20111212121212";
            string actual;
            actual = ScriptFileHelper.GetScriptFileTimeStamp(scriptFileName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptFileTimeStamp_WrongFile()
        {
            string scriptFileName = "bogus.sql";
            string expected = "bogus";
            string actual;
            actual = ScriptFileHelper.GetScriptFileTimeStamp(scriptFileName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptFileTimeStamp_TwoUnderscores()
        {
            string scriptFileName = "db__20111212121212.sql";
            string expected = "20111212121212";
            string actual;
            actual = ScriptFileHelper.GetScriptFileTimeStamp(scriptFileName);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptPath_CurrentDir()
        {
            ArgumentParser arguments = new ArgumentParser(null); 
            string expected = Environment.CurrentDirectory;
            string actual;
            actual = ScriptFileHelper.GetScriptPath(arguments);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptPath_PathSpecified()
        {
            ArgumentParser arguments = new ArgumentParser(null);
            arguments.Parameters.Add("path", @"c:\temp");
            string expected = @"c:\temp";
            string actual;
            actual = ScriptFileHelper.GetScriptPath(arguments);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptPath_PathEmpty()
        {
            ArgumentParser arguments = new ArgumentParser(null);
            arguments.Parameters.Add("path", @"");
            string expected = Environment.CurrentDirectory;
            string actual;
            actual = ScriptFileHelper.GetScriptPath(arguments);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetScriptPath_PathNull()
        {
            ArgumentParser arguments = new ArgumentParser(null);
            arguments.Parameters.Add("path", null);
            string expected = Environment.CurrentDirectory;
            string actual;
            actual = ScriptFileHelper.GetScriptPath(arguments);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetScriptFiles_Expected_3_Files()
        {
            var actual = ScriptFileHelper.GetScriptFiles(Environment.CurrentDirectory);
            var expected = new List<string>();
            expected.Add(Path.Combine(Environment.CurrentDirectory, "db_tbl_20110101010101.sql"));
            expected.Add(Path.Combine(Environment.CurrentDirectory, "db_tbl_20110202020202.sql"));
            expected.Add(Path.Combine(Environment.CurrentDirectory, "db_tbl_20110303030303.sql"));

            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
        }
    }
}
