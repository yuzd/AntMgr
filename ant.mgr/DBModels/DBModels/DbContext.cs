using AntData.ORM.Data;
using System;
using System.Diagnostics;
using System.Linq;
using Infrastructure.Logging;
using Infrastructure.Web;

namespace DbModel
{
    public class DbContext
    {
        public static MysqlDbContext<AntEntity> DB
        {
            get
            {
                var db = new MysqlDbContext<AntEntity>("ant_mysql");
#if DEBUG
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
#endif
                return db;
            }
        }

        /// <summary>
        /// 记录sql
        /// </summary>
        /// <param name="customerTraceInfo"></param>
        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = Environment.NewLine + "#####################################################" + Environment.NewLine + customerTraceInfo.SqlText;
                try
                {
                    sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
                        (current, item) => current.Replace(item.Key, item.Value.Value.ToString()));
                }
                catch (Exception)
                {
                    //ignore
                }
                sql += Environment.NewLine;
                foreach (var detail in customerTraceInfo.RunTimeList)
                {
                    
                    var sencond = (int)detail.Duration.TotalSeconds;
                    var time = sencond + "秒";
                    if (sencond < 1)
                    {
                        time = detail.Duration.TotalMilliseconds + "豪秒";
                    }
                    sql += $"Server：{detail.Server},DB名称：{detail.DbName}, 执行时间：{time}" +Environment.NewLine + "#####################################################" + Environment.NewLine;
                    LogHelper.Info("SQL",sql);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
