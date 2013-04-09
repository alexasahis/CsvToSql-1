using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using GenericParsing;

namespace CsvToSql.Commands
{
    public class CommandRunner
    {
        private Command _command;
        private SqlCommand _sqlCommand;


        public void Initialise(Command command)
        {
            _sqlCommand = new SqlCommand();
            _command = command;


            _sqlCommand.CommandText = _command.Sql;
            _sqlCommand.Connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

            //create parameters
            foreach (var param in _command.Parameters)
            {
                var length = param.Type.GetSqlLength();
                var type = param.Type.ParseDbType();

                if (length == null)
                    _sqlCommand.Parameters.Add(param.Name, type);
                else
                    _sqlCommand.Parameters.Add(param.Name, type, length.Value);
            }
        }



        public void SetParameters(GenericParser parser)
        {
            foreach (var param in _command.Parameters)
            {
                if(parser[param.DatafileColumn] == null)
                    _sqlCommand.Parameters[param.Name].Value = DBNull.Value;
                else
                {
                    _sqlCommand.Parameters[param.Name].Value = parser[param.DatafileColumn].SqlValue(_sqlCommand.Parameters[param.Name].SqlDbType);
                }
            }
        }

        public virtual string FailMessage(GenericParser parser)
        {
            return string.Format("Could not find {0} : {1}", TypeName, GetId(parser));
        }

        public virtual string SuccessMessage(GenericParser parser)
        {
            return string.Format("UPDATED {0} : Id {1}", _command.Datafile, GetId(parser));
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



                _sqlCommand.Connection.Open();

                var count = 0;

                while (parser.Read())
                {
                    SetParameters(parser);
                    count++;

                    Exception exception = null;
                    var affected = 0;
                    try
                    {
                        affected = _sqlCommand.ExecuteNonQuery();
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

                _sqlCommand.Connection.Close();


            }

        }
    }
}