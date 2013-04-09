using System.Collections.Generic;
using System.Data;
using CsvToSql.Commands;

namespace CsvToSql.DbAdaptors
{
    

    /// <summary>
    //your adaptor is chosen by naming convention.
    // speficy "DbProvider" in App.Config - and Begin the title of the adaptor with this String...
    // e.g. AppSettings["MsSqlServer"] ===> MsSqlServerAdaptor
    //
    //MUST HAVE PARAMETERLESS CONSTRUCTOR
    /// </summary>
    public interface IDbAdaptor
    {
        //create connection, but do not open
        IDbConnection CreateCommand(string connectionString, string commandSql);
        
        //should create locally stored parameters for a parameterised query
        void CreateParameters(IList<CommandParameter> commandParameters);

        //should set values of previously created parameter...
        //value should be parsed into desired type
        void SetParameter(string parameterName, string value);

        //should return no of rows affected
        int Execute();

      }
}