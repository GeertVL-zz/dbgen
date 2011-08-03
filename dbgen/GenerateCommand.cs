using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace dbgen
{
    internal class GenerateCommand
    {
        public void execute(ArgumentParser arguments)
        {
            if (arguments.DetailCommand == "file" || string.IsNullOrEmpty(arguments.DetailCommand))
            {
                this.CreateFile("general", null);
            }
            else if (arguments.DetailCommand == "create")
            {
                CreateAction(arguments.Parameters["type"], arguments.Parameters["name"], null);
            }
            else if (arguments.DetailCommand == "change")
            {
                ChangeAction(arguments.Parameters["type"], arguments.Parameters["tbl"], arguments.Parameters["name"], arguments.Parameters["args"]);
            }
            else if (arguments.DetailCommand == "drop")
            {
                DropAction(arguments.Parameters["tbl"]);
            }
            else if (arguments.DetailCommand == "data")
            {
                string connectionStringName = "db_develop";
                if (arguments.Parameters.ContainsKey("con"))
                {
                    connectionStringName = arguments.Parameters["con"];
                }
                DataAction(arguments.Parameters["tbl"], connectionStringName);
            }
            else
            {
                Console.WriteLine("Unknown action for command Generate");
            }
        }

        private string CreateFile(string generateType, List<string> contentLines)
        {
            string fileName = string.Format("db_{0}_{1:yyyyMMddHHmmss}.sql", generateType, DateTime.Now);
            using (var generatedFile = File.Create(fileName))
            {
                using (var sw = new StreamWriter(generatedFile))
                {
                    if (contentLines != null)
                    {
                        foreach (string line in contentLines)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }
            Console.WriteLine("File {0} generated.", fileName);

            return fileName;
        }

        private void CreateAction(string creationType, string name, string arguments)
        {
            if (creationType == "table")
            {
                //TODO : Implement arguments
                List<string> content = new List<string>();
                content.Add(string.Format("CREATE TABLE [dbo].[{0}] (", name));
                content.Add("\t[Id]\t\tINT\t\t\tNOT NULL,");
                content.Add("\t[Name]\t\tNVARCHAR(50)\t\t\tNOT NULL");
                content.Add(");");
                CreateFile("create_table", content);
            }
            else if (creationType == "FK")
            {
                List<string> content = new List<string>();
                content.Add("ALTER TABLE [dbo].[INSERT_TABLE_HERE]");
                content.Add(string.Format("ADD CONSTRAINT [{0}] FOREIGN KEY ([COLUMN_HERE]) REFERENCES [dbo].[INSERT_TABLE_HERE] ([COLUMN_HERE]) ON DELETE NO ACTION ON UPDATE NO ACTION;", name));
                CreateFile("create_fk", content);
            }
            else if (creationType == "CONSTRAINT")
            {
                List<string> content = new List<string>();
                content.Add("ALTER TABLE [dbo].[INSERT_TABLENAME_HERE]");
                content.Add(string.Format("\tADD CONSTRAINT [{0}] DEFAULT INSERT_CONSTRAINT_DEFINITION", name));
                CreateFile("create_constraint", content);
            }
            else if (creationType == "IDX")
            {
                List<string> content = new List<string>();
                content.Add(string.Format("CREATE UNIQUE NONCLUSTERED INDEX [{0}]", name));
                content.Add("\t\tON [dbo].[INSERT_TABLE_NAME]([INSERT_COLUMN_HERE] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)");
                content.Add("\t\tON [PRIMARY];");
                CreateFile("create_idx", content);
            }
            else if (creationType == "PK")
            {
                List<string> content = new List<string>();
                content.Add("ALTER TABLE [dbo].[INSERT_TABLENAME_HERE]");
                content.Add(string.Format("\t\tADD CONSTRAINT [PK_INSERT_TABLENAME_HERE_{0}] PRIMARY KEY CLUSTERED ([{0}] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);", name));
                CreateFile("create_pk", content);
            }
            else
            {
                // TODO : Show all creation options here.
            }
        }

        private void ChangeAction(string changeType, string tablename, string name, string arguments)
        {
            if (changeType == "add")
            {
                List<string> content = new List<string>();
                content.Add(string.Format("ALTER TABLE [dbo].[{0}] ADD", tablename));
                content.Add(string.Format("\t{0} {1}", name, arguments));
                CreateFile("change_add", content);
            }
            else if (changeType == "mod")
            {
                List<string> content = new List<string>();
                content.Add(string.Format("ALTER TABLE [dbo].[{0}] ADD", tablename));
                content.Add(string.Format("\tALTER COLUMN {0} {1}", name, arguments));
                CreateFile("change_mod", content);
            }
            else if (changeType == "drop")
            {
                List<string> content = new List<string>();
                content.Add(string.Format("ALTER TABLE [dbo].[{0}] ADD", tablename));
                content.Add(string.Format("\tDROP COLUMN {0}", name));
                CreateFile("change_drp", content);
            }
            else
            {
                // TODO : Show all creation options here.
            }
        }

        private void DropAction(string tablename)
        {
            List<string> content = new List<string>();
            content.Add(string.Format("DROP TABLE [dbo].[{0}]", tablename));
            CreateFile("drop", content);
        }

        private void DataAction(string tablename, string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(string.Format("select * from {0}", tablename), connection);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                //define columns
                string columnDefinition = string.Empty;
                foreach (DataColumn column in table.Columns)
                {
                    columnDefinition += string.Format("[{0}],", column.ColumnName);
                }
                columnDefinition = columnDefinition.Remove(columnDefinition.Length - 1, 1);

                List<string> content = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    string rowDefinition = string.Empty;
                    row.ItemArray.ToList().ForEach(i => rowDefinition += string.Format("'{0}',", ResetAliens(i.ToString())));
                    rowDefinition = rowDefinition.Remove(rowDefinition.Length - 1, 1);
                    string insertLine = string.Format("insert into {0} ({1}) values ({2})", tablename, columnDefinition, rowDefinition);
                    content.Add(insertLine);
                }

                CreateFile("data", content);
            }
        }

        private string ResetAliens(string originalValue)
        {
            return originalValue.Replace("'", "''");
        }
    }
}
