using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CsvToSql.DbAdaptors;
using GenericParsing;

namespace CsvToSql.Commands
{
    public class CommandRunner
    {
        private Command _command;
        private IDbConnection _connection;
        private IDbAdaptor _adaptor;

        public void Initialise(Command command, IDbAdaptor adaptor)
        {
            _adaptor = adaptor;
            _command = command;

            _connection = _adaptor.CreateCommand(ConfigurationManager.AppSettings["ConnectionString"], _command.Sql);

            _adaptor.CreateParameters(_command.Parameters);
        }


        
        public void SetParameters(GenericParser parser)
        {
            foreach (var param in _command.Parameters)
            {
                _adaptor.SetParameter(param.Name, parser[param.DatafileColumn]);
            }
        }

        public virtual string FailMessage(GenericParser parser)
        {
            return string.Format("NO ROWS AFFECTED {0} : {1}", _command.Datafile, GetId(parser));
        }

        public virtual string SuccessMessage(GenericParser parser)
        {
            return string.Format("PROCESSED {0} : Id {1}", _command.Datafile, GetId(parser));
        }



        public virtual string GetId(GenericParser parser)
        {

            var identity = _command.Parameters.FirstOrDefault(p => p.IsIdentity);

            if (identity != null) return parser[identity.DatafileColumn];

            return parser["ID"] ?? parser["id"] ?? parser["Id"];
        }


        public virtual string TypeName { get { return GetType().Name; } }

        public IEnumerable<string> Run()
        {

            var onlyLogErrors = bool.Parse(ConfigurationManager.AppSettings["OnlyLogErrors"]);

            using (GenericParser parser = new GenericParser())
            {
                parser.SetDataSource(_command.Datafile);

                parser.ColumnDelimiter = ',';
                parser.FirstRowHasHeader = true;
                parser.SkipStartingDataRows = 0;
                parser.MaxBufferSize = 4096;
                parser.MaxRows = 1000000;
                parser.TextQualifier = '\"';
                
                _connection.Open();

                var count = 0;

                while (parser.Read())
                {
                    SetParameters(parser);
                    count++;

                    Exception exception = null;
                    var affected = 0;
                    try
                    {
                        affected = _adaptor.Execute();
                    }
                    catch (Exception ex)
                    {

                        exception = ex;
                    }


                    if (exception != null)
                        yield return exception.ToString();
                    else if (affected == 0)
                        yield return count.ToString() + " : " + FailMessage(parser);
                    else if (!onlyLogErrors)
                        yield return count.ToString() + " : " + SuccessMessage(parser);
                    else
                        yield return "";

                }

                _connection.Close();


            }

        }
    }
}