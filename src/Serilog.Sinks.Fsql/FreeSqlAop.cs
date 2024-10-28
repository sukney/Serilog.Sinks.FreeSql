using FreeSql.Aop;

namespace Serilog.Sinks.Fsql;

/// <summary>
/// FreeSqlAop扩展方法
/// </summary>
public class FreeSqlAop
{
	/// <summary>
	/// 设置FreeSqlAop
	/// </summary>
	/// <param name="freeSql"></param>
	/// <param name="options"></param>
	public static void SetAop(IFreeSql<SerilogFsqlTag> freeSql, FreeSqlSinkOptions options)
	{
		((IFreeSql)freeSql).Aop.ConfigEntity += delegate(object? s, ConfigEntityEventArgs e)
		{
			if (e.EntityType.Name == "SerilogModel")
			{
				e.ModifyResult.Name = options.TableName;
			}
		};
		if (options.ColumnNameStyle == 2)
		{
			((IFreeSql)freeSql).Aop.ConfigEntityProperty += delegate(object? s, ConfigEntityPropertyEventArgs e)
			{
				string name2 = e.Property.Name;
				if (name2 != null)
				{
					switch (name2.Length)
					{
					case 9:
						switch (name2[1])
						{
						case 'i':
							if (name2 == "Timestamp")
							{
								e.ModifyResult.Name = "timestamp";
							}
							break;
						case 'x':
							if (name2 == "Exception")
							{
								e.ModifyResult.Name = "exception";
							}
							break;
						case 'v':
							if (name2 == "EventName")
							{
								e.ModifyResult.Name = "event_name";
							}
							break;
						}
						break;
					case 3:
						switch (name2[0])
						{
						case 'A':
							if (name2 == "App")
							{
								e.ModifyResult.Name = "app";
							}
							break;
						case 'E':
							if (name2 == "Env")
							{
								e.ModifyResult.Name = "env";
							}
							break;
						}
						break;
					case 16:
						if (name2 == "RenderedMesssage")
						{
							e.ModifyResult.Name = "message";
						}
						break;
					case 5:
						if (name2 == "Level")
						{
							e.ModifyResult.Name = "level";
						}
						break;
					case 10:
						if (name2 == "Properties")
						{
							e.ModifyResult.Name = "properties";
						}
						break;
					case 7:
						if (name2 == "EventId")
						{
							e.ModifyResult.Name = "event_id";
						}
						break;
					}
				}
			};
		}
		else
		{
			if (options.ColumnNameStyle != 1)
			{
				return;
			}
			((IFreeSql)freeSql).Aop.ConfigEntityProperty += delegate(object? s, ConfigEntityPropertyEventArgs e)
			{
				string name = e.Property.Name;
				if (name != null)
				{
					switch (name.Length)
					{
					case 9:
						switch (name[1])
						{
						case 'i':
							if (name == "Timestamp")
							{
								e.ModifyResult.Name = "Timestamp";
							}
							break;
						case 'x':
							if (name == "Exception")
							{
								e.ModifyResult.Name = "Exception";
							}
							break;
						case 'v':
							if (name == "EventName")
							{
								e.ModifyResult.Name = "EventName";
							}
							break;
						}
						break;
					case 3:
						switch (name[0])
						{
						case 'A':
							if (name == "App")
							{
								e.ModifyResult.Name = "App";
							}
							break;
						case 'E':
							if (name == "Env")
							{
								e.ModifyResult.Name = "Env";
							}
							break;
						}
						break;
					case 16:
						if (name == "RenderedMesssage")
						{
							e.ModifyResult.Name = "Message";
						}
						break;
					case 5:
						if (name == "Level")
						{
							e.ModifyResult.Name = "Level";
						}
						break;
					case 10:
						if (name == "Properties")
						{
							e.ModifyResult.Name = "Properties";
						}
						break;
					case 7:
						if (name == "EventId")
						{
							e.ModifyResult.Name = "EventId";
						}
						break;
					}
				}
			};
		}
	}
}
