using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CsvToSql.Commands;
using GenericParsing;

namespace CsvToSql.DbAdaptors
{
    
    public class MsSqlServerAdaptor : IDbAdaptor
    {

        private SqlCommand _sqlCommand;

        public IDbConnection CreateCommand(string connectionString, string commandSql)
        {
            _sqlCommand = new SqlCommand(commandSql);
            _sqlCommand.Connection = new SqlConnection(connectionString);

            return _sqlCommand.Connection;
        }

        public void CreateParameters(IList<CommandParameter> commandParameters)
        {
            //create parameters
            foreach (var param in commandParameters)
            {
                var length = param.Type.GetSqlLength();
                var type = param.Type.ParseDbType();

                if (length == null)
                    _sqlCommand.Parameters.Add(param.Name, type);
                else
                    _sqlCommand.Parameters.Add(param.Name, type, length.Value);
            }
        }

        public void SetParameter(string parameterName, string value)
        {
            if (value == null)
                _sqlCommand.Parameters[parameterName].Value = DBNull.Value;
            else
                _sqlCommand.Parameters[parameterName].Value = value.SqlValue(_sqlCommand.Parameters[parameterName].SqlDbType);
            
        }

        public int Execute()
        {
            return _sqlCommand.ExecuteNonQuery();
        }

        
    }
}
