using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace dbgen
{
    internal static class ScriptFileHelper
    {
        public static string GetScriptPath(ArgumentParser arguments)
        {
            string path = Environment.CurrentDirectory;
            if (arguments.Parameters.ContainsKey("path"))
            {
                if (!string.IsNullOrEmpty(arguments.Parameters["path"]))
                {
                    path = arguments.Parameters["path"];
                }
            }

            return path;
        }

        public static List<string> GetScriptFiles(string path)
        {
            List<string> fileNames = new List<string>(Directory.GetFiles(path, "*.sql"));
            fileNames.Sort(new TimestampComparer());

            return fileNames;
        }

        public static string GetScriptFileTimeStamp(string scriptFileName)
        {
            if (string.IsNullOrEmpty(scriptFileName)) return null;

            string dateString = null;
            string[] parts = scriptFileName.Split(new char[] { '_' });
            if (!string.IsNullOrEmpty(parts[parts.Length - 1]))
            {
                dateString = parts[parts.Length - 1].Remove(parts[parts.Length - 1].Length - 4);
            }

            return dateString;
        }
    }

    internal class TimestampComparer : IComparer<string>
    {
        #region IComparer<string> Members

        public int Compare(string leftFilename, string rightFilename)
        {
            DateTime leftTimestamp = VersionHelper.ConvertTimeStamp(ScriptFileHelper.GetScriptFileTimeStamp(leftFilename));
            DateTime rightTimestamp = VersionHelper.ConvertTimeStamp(ScriptFileHelper.GetScriptFileTimeStamp(rightFilename));

            return leftTimestamp.CompareTo(rightTimestamp);
        }

        #endregion
    }
}
