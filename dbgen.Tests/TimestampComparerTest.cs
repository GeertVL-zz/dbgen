using dbgen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace dbgen.Tests
{
    
    
    /// <summary>
    ///This is a test class for TimestampComparerTest and is intended
    ///to contain all TimestampComparerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimestampComparerTest
    {
        [TestMethod()]
        public void Compare_Empty()
        {
            TimestampComparer target = new TimestampComparer();
            string leftFilename = string.Empty; 
            string rightFilename = string.Empty; 
            int expected = 0; 
            int actual;
            actual = target.Compare(leftFilename, rightFilename);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Compare_Left()
        {
            TimestampComparer target = new TimestampComparer();
            string leftFilename = "db_tbl_20111212121212.sql";
            string rightFilename = "db_tbl_20111111111111.sql";
            int expected = 1;
            int actual;
            actual = target.Compare(leftFilename, rightFilename);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Compare_Right()
        {
            TimestampComparer target = new TimestampComparer();
            string leftFilename = "db_tbl_20111111111111.sql";
            string rightFilename = "db_tbl_20111212121212.sql";
            int expected = -1;
            int actual;
            actual = target.Compare(leftFilename, rightFilename);
            Assert.AreEqual(expected, actual);
        }
    }
}
