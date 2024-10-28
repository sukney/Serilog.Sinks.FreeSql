using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Fsql;
using Serilog.Sinks.PeriodicBatching;
using System;
using IBatchedLogEventSink = Serilog.Sinks.PeriodicBatching.IBatchedLogEventSink;

namespace Serilog;

public static class LoggerConfigurationFsqlExtensions
{

    /// <summary>
    /// fsql  ≈‰∆˜
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <param name="options"></param>
    /// <param name="periodicBatchingSinkOptions"></param>
    /// <param name="restrictedToMinimumLevel"></param>
    /// <param name="levelSwitch"></param>
    /// <returns></returns>
    public static LoggerConfiguration FsqlSink(
      this LoggerSinkConfiguration loggerConfiguration,
      FreeSqlSinkOptions options,
       PeriodicBatchingSinkOptions periodicBatchingSinkOptions = null,
         LogEventLevel restrictedToMinimumLevel = 0, LoggingLevelSwitch levelSwitch = null
      )
    {
        var val = new FreeSqlSink(options);

        return loggerConfiguration.Sink(new PeriodicBatchingSink((IBatchedLogEventSink)(val), periodicBatchingSinkOptions), restrictedToMinimumLevel, levelSwitch);
    }


    
}
