using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvToSql.Commands
{
    public class Command
    {

        public string Sql { get; set; }

        public string Datafile { get; set; }

        public List<CommandParameter> Parameters { get; set; }

    }
}
