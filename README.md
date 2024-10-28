 public class LogConfigure
 {

     /// <summary>
     /// 
     /// </summary>
     /// <param name="context"></param>
     /// <param name="serviceProvider"></param>
     /// <param name="logger"></param>
     /// <exception cref="Exception"></exception>
     public static void ConfigureSerilog(HostBuilderContext context, IServiceProvider serviceProvider, LoggerConfiguration logger)
     {

        
         var loggerConfiguration = logger.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(serviceProvider)
             .Enrich.FromLogContext();

         loggerConfiguration.WriteTo.Map(keyPropertyName: "Name",
        defaultKey: "0",
         configure: (name, wt) =>
         {
             wt.Async(lc => lc.Fsql(
                 options: new Serilog.Sinks.Fsql.FreeSqlSinkOptions
                 {
                     ConnectionString = "Data Source=152.136.140.48;Port=23306;User ID=root;Password=sa_123; Initial Catalog=caller;Charset=utf8; SslMode=none;Min pool size=1",
                     ColumnNameStyle = 2,
                     DataType = FreeSql.DataType.MySql,
                     EnableShard = true,
                     ShardReserveTableDay = 30,
                 }),
              100, true);

         });
     }
 }