using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbgen
{
    internal class ScriptFileName
    {
        private string _fileName;
        
        public string ScriptType { get; set; }
        public string TimeStamp { get; set; }

        public ScriptFileName(string fileName)
        {
            _fileName = fileName;
            Parse();
        }

        private void Parse()
        {
            this.TimeStamp = ScriptFileHelper.GetScriptFileTimeStamp(_fileName);

            string[] parts = _fileName.Split(new char[] { '_' });
            if (parts.Length > 2)
            {
                this.ScriptType = parts[1];
            }
        }
    }
}
