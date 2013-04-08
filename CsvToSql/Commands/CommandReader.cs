using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace CsvToSql.Commands
{
    public class CommandReader
    {
        public List<Command> GetCommands()
        {
            var commandFile = ConfigurationManager.AppSettings["CommandFile"];

            var commandJson = File.ReadAllText(commandFile);

            return JsonConvert.DeserializeObject<List<Command>>(commandJson);


        }
    }
}