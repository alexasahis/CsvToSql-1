using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CsvToSql.Commands;

namespace CsvToSql
{
    class Program
    {
        static void Main(string[] args)
        {

            var commandreader = new CommandReader();

            var commands = commandreader.GetCommands();
               
            var summaryList = new Dictionary<string, int >();

            var onlyLoggingErrors = bool.Parse(ConfigurationManager.AppSettings["OnlyLogErrors"]);

            foreach(var command in commands)
            {
                var commandRunner = new CommandRunner();
                
                commandRunner.Initialise(command);

                var logFile = new StreamWriter(command.Datafile + ".log");

                summaryList.Add(command.Datafile, 0);

                var count = 0;

                try
                {
                    foreach (var message in commandRunner.Run())
                    {
                        if(!string.IsNullOrEmpty(message)) log(logFile, message);

                        if (onlyLoggingErrors && count % 1000 == 0) Console.WriteLine("********** RECORD: {0} *************", count); 

                        count++;
                    } 
                }
                catch (Exception ex)
                {
                    log(logFile, ex.ToString());
                }

                logFile.Close();
                summaryList[command.Datafile] = count;
            }

            foreach(var summary in summaryList)
                Console.WriteLine("{0} : {1}", summary.Key, summary.Value);

            Console.ReadLine();

        }

        static void log(StreamWriter logFile, string message)
        {
            logFile.WriteLine(message);
            Console.WriteLine(message);
        }
        
    }
}
