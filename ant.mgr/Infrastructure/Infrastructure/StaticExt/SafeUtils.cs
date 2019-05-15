using System.Net;
using System.Text.RegularExpressions;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 安全检查（防止XSS跨站脚本攻击）
    /// </summary>
    /// <remarks>yuzd 2013-3-4</remarks>
    public class SafeUtils
    {
        /// <summary>
        /// 非法数据定义 GET
        /// </summary>
        private const string getRegex = "<|>|\"|'|\\b(and|or)\\b.+?(>|<|=|\\bin\\b|\\blike\\b)|\\/\\*.+?\\*\\/|<\\s*script\\b|\\bEXEC\\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\\s+(TABLE|DATABASE)";
        /// <summary>
        /// 非法数据定义 POST
        /// </summary>
        private const string postRegex = "\\b(and|or)\\b.{1,6}?(=|>|<|\\bin\\b|\\blike\\b)|\\/\\*.+?\\*\\/|<\\s*script\\b|\\bEXEC\\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\\s+(TABLE|DATABASE)";
        /// <summary>
        /// 非法数据定义 COOKIE
        /// </summary>
        private const string cookieRegex = "\\b(and|or)\\b.{1,6}?(=|>|<|\\bin\\b|\\blike\\b)|\\/\\*.+?\\*\\/|<\\s*script\\b|\\bEXEC\\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\\s+(TABLE|DATABASE)";

        public static bool CheckIsNotSafeString(string target)
        {

            if (Regex.IsMatch(target, cookieRegex))
            {
                //Utils.WriteErrorLog(WebRequest.GetIP() + " 提交中有非法数据 " + inputData);
                return true;
            }
            else
            {
                target = WebUtility.UrlDecode(target);
                return Regex.IsMatch(target, cookieRegex);
            }
        }

    }
}
