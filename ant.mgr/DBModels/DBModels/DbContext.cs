using AntData.ORM.Data;
using System;
using System.Diagnostics;
using System.Linq;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.Web;

namespace DbModel
{
    public class DbContext
    {
        /// <summary>
        /// 后台系统采用的是什么数据库
        /// </summary>
        private static readonly string dbType;
        static DbContext()
        {

            try
            {
                //读取当前采用的是什么数据库
                dbType = ConfigHelper.GetConfig<string>("AntDbType");
            }
            catch (Exception)
            {
                dbType = "mysql";
            }

            if (string.IsNullOrEmpty(dbType)) dbType = "mysql";
        }
        public static DbContext<AntEntity> DB
        {
            get
            {
                DbContext<AntEntity> db;
                if (dbType.ToLower().Equals("mysql"))
                {
                    db = new MysqlDbContext<AntEntity>("ant_mysql");
                }
                else
                {
                    db = new SqlServerlDbContext<AntEntity>("ant_sqlserver");
                }
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
                    sql += $"Server：{detail.Server},DB名称：{detail.DbName}, 执行时间：{time}" + Environment.NewLine + "#####################################################" + Environment.NewLine;
                    LogHelper.Info("SQL", sql);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
