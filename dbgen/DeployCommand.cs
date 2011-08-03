using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Transactions;

namespace dbgen
{
    public class DeployCommand
    {
        private string _connectionString;
        private string _connectionStringName;

        public bool DeployData { get; set; }

        public DeployCommand(string connectionStringName)
        {
            DeployData = false;
            _connectionStringName = connectionStringName;
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        /// <summary>
        /// Deploys only one file 
        /// </summary>
        /// <param name="connectionstringName"></param>
        /// <param name="deployFile"></param>
        public void Execute(string deployFile)
        {            
            if (!string.IsNullOrEmpty(deployFile))
            {
                Console.WriteLine("Trying to deploy {0}", new FileInfo(deployFile).Name);
                ExecuteFile(deployFile);
            }
        }

        /// <summary>
        /// Deploys everything in order
        /// </summary>
        /// <param name="connectionStringName"></param>
        public void Execute()
        {
            List<string> fileNames = new List<string>(Directory.GetFiles(Environment.CurrentDirectory, "*.sql"));
            ExecuteFiles(fileNames);
        }

        /// <summary>
        /// Deploys all files found from the version timestamp
        /// </summary>
        /// <param name="connectionstringName"></param>
        /// <param name="version"></param>
        public void Execute(DateTime versionDate)
        {
            List<string> fileNames = new List<string>(Directory.GetFiles(Environment.CurrentDirectory, "*.sql"));
            List<string> afterVersion = new List<string>();
            foreach (string fileName in fileNames)
            {
                DateTime fileDate = VersionHelper.ConvertTimeStamp(ScriptFileHelper.GetScriptFileTimeStamp(fileName));
                if (fileDate > versionDate)
                {
                    afterVersion.Add(fileName);
                }
            }

            fileNames.Clear();
            fileNames.AddRange(afterVersion);

            ExecuteFiles(fileNames);
        }

        private int ExecuteFiles(List<string> fileNames)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                bool rollback = false;
                foreach (string fileName in fileNames)
                {
                    if (DeployData || new ScriptFileName(fileName).ScriptType != "data")
                    {
                        Console.WriteLine("Trying to deploy {0}", new FileInfo(fileName).Name);
                        if (ExecuteFile(fileName) < 0)
                        {
                            rollback = true;
                        }
                    }
                }

                if (!rollback && fileNames.Count > 0)
                {
                    string lastTimestamp = ScriptFileHelper.GetScriptFileTimeStamp(fileNames[fileNames.Count - 1]);
                    VersionHelper.SetDbConfigVersion(_connectionStringName, lastTimestamp);
                    scope.Complete();                    
                    Console.WriteLine("Deployment of file(s) succeeded.");
                    Console.WriteLine("Version deployed on database {0} with new version {1}", _connectionStringName, lastTimestamp);
                    return 1;
                }
            }

            return -1;
        }

        private int ExecuteFile(string fileName)
        {
            int result = 0;

            if (File.Exists(fileName))
            {
                using (var file = File.OpenText(fileName))
                {
                    string sqlscript = file.ReadToEnd();
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        try
                        {
                            connection.Open();
                            var command = new SqlCommand(sqlscript, connection);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("{0}: {1}", fileName, ex.Message));
                            result = -1;
                        }
                    }
                }
            }

            return result;
        }
    }
}

