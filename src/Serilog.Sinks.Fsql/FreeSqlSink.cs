using FreeSql;
using FreeSql.DatabaseModel;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBatchedLogEventSink = Serilog.Sinks.PeriodicBatching.IBatchedLogEventSink;

namespace Serilog.Sinks.Fsql;

/// <summary>
/// FreeSql Sink
/// 可设置是否开启分表分库
/// 开启分表分库时，需要配置保留日志天数，定时清理日志
/// </summary>
public class FreeSqlSink : IBatchedLogEventSink
{
    private IFreeSql<SerilogFsqlTag> _fsql;

    private FreeSqlSinkOptions _options;

    private Timer _timer;

    /// <summary>
    /// FreeSqlSink
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="T:System.Exception"></exception>
    public FreeSqlSink(FreeSqlSinkOptions options)
    {
        //IL_0026: Unknown result type (might be due to invalid IL or missing references)
        //IL_002c: Unknown result type (might be due to invalid IL or missing references)
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new Exception("FsqlSQL Skin连接字符串不能为空");
        }
        _options = options;
        _fsql = new FreeSqlBuilder().UseConnectionString(options.DataType, options.ConnectionString, (Type)null).UseAutoSyncStructure(false).Build<SerilogFsqlTag>();
        FreeSqlAop.SetAop(_fsql, options);
        InitTable();
        InitClearTableTimer();
    }

    /// <summary>
    /// 启动时迁移表结构
    /// </summary>
    private void InitTable()
    {
        string text = (_options.EnableShard ? DateTime.Now.ToString("yyyyMMdd") : null);
        string text2 = ((text != null) ? (_options.TableName + "_" + text) : _options.TableName);
        ((IFreeSql)_fsql).CodeFirst.SyncStructure(typeof(SerilogModel), text2, false);
    }
    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        List<SerilogModel> list = LogBuilder.Build(batch);
        if (_options.EnableShard)
        {
            IEnumerable<IGrouping<string, SerilogModel>> enumerable = from x in list
                                                                      group x by x.Timestamp.Date.ToString("yyyyMMdd");
            foreach (IGrouping<string, SerilogModel> item in enumerable)
            {
                IInsert<SerilogModel> tempFsql = ((IFreeSql)_fsql).Insert<SerilogModel>(item.ToList()).AsTable((Func<string, string>)((string oldname) => oldname + "_" + item.Key)).NoneParameter(true);
                await SaveAsync(tempFsql, item.Key);
            }
        }
        else
        {
            IInsert<SerilogModel> tempFsql2 = ((IFreeSql)_fsql).Insert<SerilogModel>(list).NoneParameter(true);
            await SaveAsync(tempFsql2);
        }
    }


    /// <summary>
    /// OnEmptyBatchAsync
    /// </summary>
    /// <returns></returns>
    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }

    private async Task SaveAsync(IInsert<SerilogModel> tempFsql, string shardTableKey = null)
    {
        string text = ((shardTableKey != null) ? (_options.TableName + "_" + shardTableKey) : _options.TableName);
        ((IFreeSql)_fsql).CodeFirst.SyncStructure(typeof(SerilogModel), text, false);
        await tempFsql.ExecuteAffrowsAsync(default(CancellationToken));
    }

    /// <summary>
    /// 初始化Timer，用于清除旧的分表数据，每天执行一次
    /// </summary>
    private void InitClearTableTimer()
    {
        if (_options.EnableShard)
        {
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _options.ShardTableClearHour, 0, 0);
            if (DateTime.Now.Hour >= _options.ShardTableClearHour)
            {
                dateTime = dateTime.AddDays(1.0);
            }
            _timer = new Timer(ClearTable, null, dateTime.Subtract(DateTime.Now), TimeSpan.FromDays(1.0));
        }
    }

    /// <summary>
    /// 清理分表
    /// </summary>
    /// <param name="state"></param>
    private void ClearTable(object state)
    {
        //IL_0098: Unknown result type (might be due to invalid IL or missing references)
        //IL_009f: Invalid comparison between Unknown and I4
        string dbName = ((IFreeSql)_fsql).CodeFirst.GetTableByEntity(typeof(SerilogModel)).DbName;
        IEnumerable<DbTableInfo> source = from a in ((IFreeSql)_fsql).DbFirst.GetTablesByDatabase(Array.Empty<string>())
                                          where a.Name.StartsWith(dbName + "_")
                                          select a;
        DateTime dateTime = DateTime.Now.AddDays(-_options.ShardReserveTableDay);
        DateTime tempTime = dateTime.AddDays(-30.0);
        List<string> list = new List<string>();
        string text = (((int)_options.DataType == 12) ? ((IFreeSql)_fsql).DbFirst.GetDatabases()[0] : null);
        while (tempTime.Date < dateTime.Date)
        {
            DbTableInfo val = source.Where((DbTableInfo x) => x.Name == dbName + "_" + tempTime.ToString("yyyyMMdd")).FirstOrDefault();
            if (val != null)
            {
                if (text != null)
                {
                    list.Add($"\"{text}\".\"{val.Name}\"");
                }
                else
                {
                    list.Add($"\"{val.Schema}\".\"{val.Name}\"");
                }
            }
            tempTime = tempTime.AddDays(1.0);
        }
        if (list.Count > 0)
        {
            list.ForEach(delegate (string item)
            {
                ((IFreeSql)_fsql).Ado.ExecuteNonQuery("DROP TABLE IF EXISTS " + item + ";", (object)null);
            });
        }
    }


}
