using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Newtonsoft.Json;

namespace Infrastructure.Web
{
    /// <summary>
    /// 记录请求过程中的上下文信息
    /// </summary>
    public class RequestContext
    {
        public static readonly RequestContext Instance = new RequestContext();

        public static readonly AsyncLocal<ConcurrentDictionary<string, object>> AsyncRequestItems = new AsyncLocal<ConcurrentDictionary<string, object>>();

        /// <summary>
        /// 开启新一轮的记录
        /// 每个线程一个
        /// 支持异步上下文
        /// </summary>
        public void StartRequestContext()
        {
            CreateItems();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (AsyncRequestItems.Value.ContainsKey(key))
                return (T)AsyncRequestItems.Value[key];

            return default(T);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Set<T>(string key, T obj)
        {
            if (AsyncRequestItems.Value == null)
            {
                AsyncRequestItems.Value = new ConcurrentDictionary<string, object>();
            }

            if (AsyncRequestItems.Value.ContainsKey(key))
            {
                AsyncRequestItems.Value[key] = obj;
            }
            else
            {
                AsyncRequestItems.Value.AddOrUpdate(key, obj, (a, b) => b);
            }
        }

        /// <summary>
        /// 结束清空
        /// </summary>
        public void EndRequest()
        {
            AsyncRequestItems.Value?.Clear();
        }


        public override string ToString()
        {
            var sb = "";
            foreach (KeyValuePair<string, object> item in AsyncRequestItems.Value)
            {
                sb += ((item.Key ?? "空") + "=====>" + (item.Value == null ? "null" : item.Value.ToJsonString())) + Environment.NewLine;
            }

            return sb;
        }

        #region Privae



        private ConcurrentDictionary<string, object> CreateItems()
        {
            return AsyncRequestItems.Value = new ConcurrentDictionary<string, object>();
        }

        #endregion


    }
}
