using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Fsql;

/// <summary>
/// LogBuilder
/// </summary>
public class LogBuilder
{
	/// <summary>
	/// Build
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="formatProvider"></param>
	/// <returns></returns>
	public static SerilogModel Build(LogEvent logEvent, IFormatProvider formatProvider = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		SerilogModel obj = new SerilogModel
		{
			Timestamp = logEvent.Timestamp.DateTime,
			RenderedMesssage = logEvent.RenderMessage(formatProvider)
		};
		LogEventLevel level = logEvent.Level;
		obj.Level = level.ToString();
		obj.Exception = ((logEvent.Exception == null) ? null : logEvent.Exception.ToString());
		GetExtendInfo(obj, logEvent);
		obj.Properties = PropertiesToJson(logEvent);
		return obj;
	}

	/// <summary>
	/// Build
	/// </summary>
	/// <param name="events"></param>
	/// <param name="formatProvider"></param>
	/// <returns></returns>
	public static List<SerilogModel> Build(IEnumerable<LogEvent> events, IFormatProvider formatProvider = null)
	{
		List<SerilogModel> list = new List<SerilogModel>();
		foreach (LogEvent @event in events)
		{
			list.Add(Build(@event));
		}
		return list;
	}

	/// <summary>
	/// PropertiesToJson
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns></returns>
	private static string PropertiesToJson(LogEvent logEvent)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (logEvent.Properties.Count == 0)
		{
			return "{}";
		}
		JsonValueFormatter val = new JsonValueFormatter("_typeTag");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		using (StringWriter stringWriter = new StringWriter(stringBuilder))
		{
			foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
				handler.AppendLiteral("\"");
				handler.AppendFormatted(property.Key);
				handler.AppendLiteral("\":");
				stringBuilder2.Append(ref handler);
				val.Format(property.Value, (TextWriter)stringWriter);
				stringBuilder.Append(", ");
			}
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	/// <summary>
	/// 取扩展字段信息
	/// </summary>
	/// <param name="model"></param>
	/// <param name="logEvent"></param>
	private static void GetExtendInfo(SerilogModel model, LogEvent logEvent)
	{
		if (logEvent.Properties.TryGetValue("EventId", out var value))
		{
			StructureValue val = (StructureValue)(object)((value is StructureValue) ? value : null);
			if (val != null)
			{
				IReadOnlyList<LogEventProperty> properties = val.Properties;
				if (properties != null && properties.Count == 2)
				{
					LogEventProperty obj = val.Properties[0];
					if (int.TryParse((obj == null) ? null : ((object)obj.Value)?.ToString(), out var result))
					{
						model.EventId = result;
					}
					LogEventProperty obj2 = val.Properties[1];
					model.EventName = ((obj2 == null) ? null : ((object)obj2.Value)?.ToString()?.Replace("\"", ""));
				}
			}
		}
		if (logEvent.Properties.TryGetValue("app", out var value2))
		{
			model.App = ((object)value2).ToString()?.Replace("\"", "");
		}
		if (logEvent.Properties.TryGetValue("dotnet_env", out var value3))
		{
			model.Env = ((object)value3).ToString().Replace("\"", "");
		}
	}
}
