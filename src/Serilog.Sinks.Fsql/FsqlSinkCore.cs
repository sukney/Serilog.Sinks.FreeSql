using System;

namespace Serilog.Sinks.Fsql
{
    internal class FsqlSinkCore
    {
        private string connectionString;
        private IFormatProvider formatProvider;
        private FsqlSinkOptions options;
        private string tableName;
        private bool autoCreateTable;

        public FsqlSinkCore(string connectionString, IFormatProvider formatProvider, FsqlSinkOptions options, string tableName, bool autoCreateTable)
        {
            this.connectionString = connectionString;
            this.formatProvider = formatProvider;
            this.options = options;
            this.tableName = tableName;
            this.autoCreateTable = autoCreateTable;
        }
    }
}