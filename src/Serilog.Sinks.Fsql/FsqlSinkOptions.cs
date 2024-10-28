using FreeSql;

namespace Serilog.Sinks.Fsql
{
    public class FsqlSinkOptions
    {
        public DataType DataType { get; internal set; }
        public string ConnectionString { get; internal set; }
    }
}