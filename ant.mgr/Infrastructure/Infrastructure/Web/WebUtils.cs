using Infrastructure.StaticExt;
using Microsoft.AspNetCore.Http;
using System;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.RegularExpressions;

namespace Infrastructure.Web
{
    public static class WebUtils
    {

        public static string AppBaseUrl => $"{HttpContext.Current.Request.Scheme}://{HttpContext.Current.Request.Host}{HttpContext.Current.Request.PathBase}";

        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 原本的GetDisplayUrl 在请求的时候Host没有值回报错
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string SafeGetDisplayUrl(this HttpRequest request)
        {
            try
            {
                return request.GetDisplayUrl();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
            {
                var isHxr = request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isHxr)
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(request.ContentType))
            {
                return request.ContentType.ToLower().Equals("application/x-www-form-urlencoded") ||
                       request.ContentType.ToLower().Equals("application/json");
            }

            return false;
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName];
            }

            return string.Empty;
        }

        public static void CookieClear()
        {
            foreach (string cookie in HttpContext.Current.Request.Cookies.Keys)
            {
                HttpContext.Current.Response.Cookies.Delete(cookie);
            }
        }

        /// <summary>
        /// 写cookie值（未设置过期时间，则写的是浏览器进程Cookie，一旦浏览器（是浏览器，非标签页）关闭，则Cookie自动失效）
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            var cookieOptions = new CookieOptions {HttpOnly = true};
            HttpContext.Current.Response.Cookies.Append(strName, strValue, cookieOptions);
        }


        /// <summary>
        /// 获取客户端浏览器的原始用户代理信息
        /// </summary>
        /// <returns></returns>
        public static string GetUserAgent()
        {
            return HttpContext.Current.Request.Headers["User-Agent"].ToString();
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        private static string GetIP()
        {
            var result = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Connection.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrEmpty(result) || !Regex.IsMatch(result, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                return "127.0.0.1";
            }

            return result;
        }

        /// <summary>
        /// 对于使用集群负载均衡的时候获取真实客户的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string str = string.Empty;
            try
            {
                str = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                if (string.IsNullOrEmpty(str))
                {
                    return GetIP();
                }
            }
            catch
            {
                str = "";
            }

            return str;
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }

            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

    }
}