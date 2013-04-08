namespace CsvToSql.Commands
{
    public class CommandParameter
    {
        public string Name { get; set; }

        public string Type { get; set; }

        private string _datafileColumn;
        public string DatafileColumn
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_datafileColumn))
                    return Name.Replace("@", "");

                return _datafileColumn;
            }
            set { _datafileColumn = value; }
        }

        public bool IsIdentity { get; set; }
    }
}