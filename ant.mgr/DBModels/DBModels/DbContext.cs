using AntData.ORM.Data;
using System;
using System.Diagnostics;
using System.Linq;
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
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
                return db;
            }
        }

        /// <summary>
        /// 记录sql
        /// </summary>
        /// <param name="customerTraceInfo"></param>
        public static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            string sql = customerTraceInfo.SqlText;
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
                sql += $"Server：{detail.Server},DB名称：{detail.DbName}, 执行时间：{detail.Duration.TotalSeconds}秒" +Environment.NewLine ;
                Debug.Write(sql);
            }
        }
    }
}
