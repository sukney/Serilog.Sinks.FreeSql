using FreeSql;

namespace Serilog.Sinks.Fsql;

/// <summary>
/// FreeSqlSinkOptions
/// </summary>
public class FreeSqlSinkOptions
{
	/// <summary>
	/// 连接字符串
	/// </summary>
	public string ConnectionString { get; set; }

	/// <summary>
	/// 使用PgCopy
	/// </summary>
	public bool UseBulk { get; set; } = true;


	/// <summary>
	/// 是否开启分表
	/// </summary>
	public bool EnableShard { get; set; }

	/// <summary>
	/// 每天4点执行分表清理
	/// </summary>
	public int ShardTableClearHour { get; set; } = 4;


	/// <summary>
	/// 分表保留天数
	/// </summary>
	public int ShardReserveTableDay { get; set; } = 15;


	/// <summary>
	/// 表名，默认为小写log
	/// </summary>
	public string TableName { get; set; } = "log";


	/// <summary>
	/// 表字段名称风格：1大驼峰，2小写+下划线
	/// </summary>
	public int ColumnNameStyle { get; set; } = 2;


	/// <summary>
	/// 数据库类型(默认是PostgreSQL)
	/// </summary>
	public DataType DataType { get; set; } = (DataType)2;

}
