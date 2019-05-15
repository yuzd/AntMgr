using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Infrastructure.StaticExt;
using NLog;

namespace Infrastructure.Logging
{
    public static class LogHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static LogHelper()
        {

        }

        public static void Error(string title, Exception exception, [CallerMemberName]string methodName = "")
        {
            logger.Error(exception, title.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
        }

        public static void Error(string title, string msg, [CallerMemberName]string methodName = "")
        {
            logger.Error(title + msg.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
        }

        /// <summary>
        ///  调用外部接口 出错 统一用Warn
        /// </summary>
        /// <param name="title"></param>
        /// <param name="exception"></param>
        /// <param name="methodName"></param>
        public static void Warn(string title, Exception exception, [CallerMemberName]string methodName = "")
        {
            logger.Warn(exception, title.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));

        }

        /// <summary>
        /// 调用外部接口 出错 统一用Warn
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="methodName"></param>
        public static void Warn(string title, string msg, [CallerMemberName]string methodName = "")
        {
            logger.Warn(title + msg.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
        }

        public static void Warn(string title, string msg, Exception exception, [CallerMemberName]string methodName = "")
        {
            logger.Warn(exception, title + msg.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));

        }
        public static void Warn(string title, string msg, Dictionary<string, string> addInfo, [CallerMemberName]string methodName = "")
        {
            if (addInfo != null)
            {
                addInfo.Add("method", methodName);
                logger.Warn(title + msg.AppendDic(addInfo));
            }
            else
            {
                logger.Warn(title + msg.AppendDic(new Dictionary<string, string>
                {
                    {"method",methodName}
                }));
            }
        }
        public static void Warn(string title, Exception ex, Dictionary<string, string> addInfo, [CallerMemberName]string methodName = "")
        {
            if (addInfo != null)
            {
                addInfo.Add("method", methodName);
                logger.Warn(ex, title.AppendDic(addInfo));
            }
            else
            {
                logger.Warn(ex, title.AppendDic(new Dictionary<string, string>
                {
                    {"method",methodName}
                }));
            }
        }
        /// <summary>
        /// 内部接口出错 或者 打Debug日志 用Info
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="methodName"></param>
        /// <param name="addInfo"></param>
        public static void Info(string title, string message, Dictionary<string, string> addInfo = null, [CallerMemberName]string methodName = "")
        {
            if (addInfo != null)
            {
                addInfo.Add("method", methodName);
                logger.Info(title + message.AppendDic(addInfo));
            }
            else
            {
                logger.Info(title + message.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
            }
        }

        public static void Debug(string title, string message = "", Dictionary<string, string> addInfo = null, [CallerMemberName]string methodName = "")
        {
            if (addInfo != null)
            {
                addInfo.Add("method", methodName);
                logger.Debug(title + message.AppendDic(addInfo));
            }
            else
            {
                logger.Debug(title + message.AppendDic(new Dictionary<string, string>
                {
                    {"method",methodName}
                }));
            }
        }

        /// <summary>
        /// 内部接口出错 或者 打Debug日志 用Info
        /// </summary>
        /// <param name="title"></param>
        /// <param name="exception"></param>
        /// <param name="methodName"></param>
        public static void Info(string title, Exception exception, [CallerMemberName]string methodName = "")
        {
            logger.Info(exception, title.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
        }

        public static void Debug(string title, Exception exception, [CallerMemberName]string methodName = "")
        {
            logger.Debug(exception, title.AppendDic(new Dictionary<string, string>
            {
                {"method",methodName}
            }));
        }

    }
}

