using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbgen
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new ArgumentParser(args);
            string connectionStringName = "db_develop";
            if (arguments.Parameters.ContainsKey("con"))
            {
                connectionStringName = arguments.Parameters["con"];
            }

            if (arguments.Command == "generate")
            {
                // expected args[1] - gentype (file/create/change/remove/data)
                // create sqlscript with proper timestamp
                new GenerateCommand().execute(arguments);
            }
            else if (arguments.Command == "deploy")
            {
                var command = new DeployCommand(connectionStringName);
                if (arguments.Parameters.ContainsKey("data"))
                {
                    command.DeployData = true;
                }
                // expected args[1] - dbconname ; args[2] - van welke file of date
                // deploy de beschikbare scripts naar de db
                if (arguments.DetailCommand == "reset")
                {
                    // dbgen deploy reset (-con:connectionStringName)
                    command.Execute();
                }
                else if (arguments.Parameters.ContainsKey("file"))
                {
                    // dbgen deploy -file:deployfile.sql (-con:connectionStringName)
                    command.Execute(arguments.Parameters["file"]);
                }
                else
                {
                    // dbgen deploy (-con:connectionStringName)
                    DateTime version = VersionHelper.GetDbConfigVersionDate(connectionStringName);
                    if (version == DateTime.MinValue)
                    {
                        command.Execute();
                    }
                    else
                    {
                        command.Execute(version);
                    }
                }
            }
            else if (arguments.Command == "version")
            {
                // dbgen version (-con:connectionStringName)
                new VersionCommand().Execute(connectionStringName);
            }
        }
    }
}
