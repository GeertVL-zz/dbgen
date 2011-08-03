using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace dbgen
{
    internal class VersionHelper
    {
        internal static DateTime ConvertTimeStamp(string timestamp)
        {
            if (!string.IsNullOrEmpty(timestamp) && timestamp.Length == 14)
            {
                int year = Convert.ToInt32(timestamp.Substring(0, 4));
                int month = Convert.ToInt32(timestamp.Substring(4, 2));
                int day = Convert.ToInt32(timestamp.Substring(6, 2));
                int hour = Convert.ToInt32(timestamp.Substring(8, 2));
                int minute = Convert.ToInt32(timestamp.Substring(10, 2));
                int second = Convert.ToInt32(timestamp.Substring(12, 2));

                return new DateTime(year, month, day, hour, minute, second);
            }

            return DateTime.MinValue;
        }

        internal static DateTime GetDbConfigVersionDate(string connectionstringName)
        {
            string version = VersionHelper.GetDbConfigVersion(connectionstringName);

            return VersionHelper.ConvertTimeStamp(version);
        }

        internal static string GetDbConfigVersion(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select Value from Configs where Name = 'DbVersion'", connection);
                    var version = command.ExecuteScalar();
                    connection.Close();

                    if (version != null)
                    {
                        return version.ToString();
                    }
                }
            }

            return null;
        }

        internal static void SetDbConfigVersion(string connectionStringName, string version)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                string oldVersion = VersionHelper.GetDbConfigVersion(connectionStringName);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = null;
                    if (string.IsNullOrEmpty(oldVersion))
                    {
                        command = new SqlCommand(string.Format("insert into Configs (ConfigId, Name, Value) values (555, 'DbVersion', '{0}')", version), connection);
                    }
                    else
                    {
                        command = new SqlCommand(string.Format("update Configs set Value = '{0}' where Name = 'DbVersion'", version), connection);
                    }
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}
