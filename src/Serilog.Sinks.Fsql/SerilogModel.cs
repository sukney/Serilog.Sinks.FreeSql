using System;
using FreeSql.DataAnnotations;

namespace Serilog.Sinks.Fsql;

/// <summary>
/// 日志实体
/// </summary>
[Index("idx_{TableName}_01", "Timestamp")]
[Index("idx_{TableName}_02", "Level")]
public class SerilogModel
{
	/// <summary>
	/// Messsage
	/// </summary>
	[Column(StringLength = -2)]
	public string RenderedMesssage { get; set; }

	/// <summary>
	/// Level
	/// </summary>
	[Column(StringLength = 50)]
	public string Level { get; set; }

	/// <summary>
	/// EventId，默认为0
	/// </summary>
	public int? EventId { get; set; } = 0;


	/// <summary>
	/// EventName
	/// </summary>
	[Column(StringLength = 100)]
	public string EventName { get; set; }

	/// <summary>
	/// 应用名称
	/// </summary>
	[Column(StringLength = 100)]
	public string App { get; set; }

	/// <summary>
	/// Env环境
	/// </summary>
	[Column(StringLength = 50)]
	public string Env { get; set; }

	/// <summary>
	/// Timestamp
	/// </summary>
	public DateTime Timestamp { get; set; }

	/// <summary>
	/// Exception
	/// </summary>
	[Column(StringLength = -2)]
	public string Exception { get; set; }

	/// <summary>
	/// Properties
	/// </summary>
	[Column(StringLength = -2)]
	public string Properties { get; set; }
}
