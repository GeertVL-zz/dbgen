using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace dbgen
{
    internal class VersionCommand
    {
        public void Execute(string connectionStringName)
        {
            if (!string.IsNullOrEmpty(connectionStringName))
            {
                string version = VersionHelper.GetDbConfigVersion(connectionStringName);
                Console.WriteLine("Database version of {0} : {1}", connectionStringName, version);
            }
            else
            {
                Console.WriteLine("Please specify the database connectionname to get the version.");
            }
        }
    }
}
