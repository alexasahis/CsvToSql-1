using System;
using System.Data;
using System.Linq;

namespace CsvToSql.DbAdaptors
{
    public static class SqlStringExtensions
    {
        public static SqlDbType ParseDbType(this string typeStr)
        {
            var allTypes = Enum.GetNames(typeof(SqlDbType))
                .ToDictionary(n => Enum.Parse(typeof(SqlDbType), n), n => n);


            if (typeStr.Contains("("))
                typeStr = typeStr.Substring(0, typeStr.IndexOf("("));


            var dbType = allTypes.FirstOrDefault(t => t.Value.ToLower() == typeStr.ToLower());

            //            if (dbType == null) return null;

            return (SqlDbType)dbType.Key;
        }

        public static int? GetSqlLength(this string typeStr)
        {
            if (!typeStr.Contains("(")) return null;

            var start = typeStr.IndexOf("(");
            var end = typeStr.IndexOf(")");

            return int.Parse(typeStr.Substring(start + 1, end - start - 1));
        }


        public static object SqlValue(this string strVal, SqlDbType type)
        {
            if (type == SqlDbType.DateTime)
                return DateTime.Parse(strVal);

            return strVal;

        }

    }
}