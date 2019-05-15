using Infrastructure.StaticExt;
using Microsoft.AspNetCore.Http;
using System;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using System.IO;
using System.Net;
using System.Threading;
using Infrastructure.Logging;
using System.Text;

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

            if (string.IsNullOrEmpty(result) || !ValidatorUtils.IsIPv4(result))
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

        /// <summary>
        /// 获取View页面的绝对路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static string GetViewMapPath(string strPath)
        {
            strPath = strPath.Replace("~", "~/Views") + ".cshtml";
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }

            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

        /// <summary>
        /// Http方式下载文件
        /// </summary>
        /// <param name="url">http地址</param>
        /// <param name="localFile">本地文件</param>
        /// <returns></returns>
        public static bool Download(string url, string localFile)
        {
            bool flag = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            long remoteFileLength = GetHttpLength(url);// 取得远程文件长度
            // System.Console.WriteLine("remoteFileLength=" + remoteFileLength);
            if (remoteFileLength == 745)
            {
                //Console.WriteLine("远程文件不存在.");
                return false;
            }

            // 判断要下载的文件夹是否存在
            if (File.Exists(localFile))
            {

                writeStream = File.OpenWrite(localFile); // 存在则打开要下载的文件
                startPosition = writeStream.Length; // 获取已经下载的长度

                if (startPosition >= remoteFileLength)
                {
                    // Console.WriteLine("本地文件长度" + startPosition + "已经大于等于远程文件长度" + remoteFileLength);
                    writeStream.Close();

                    return false;
                }
                else
                {
                    writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
                }
            }
            else
            {
                writeStream = new FileStream(localFile, FileMode.Create);// 文件不保存创建一个文件
                startPosition = 0;
            }


            try
            {
                HttpWebRequest myRequest = (HttpWebRequest) HttpWebRequest.Create(url); // 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int) startPosition); // 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }

                myRequest.ReadWriteTimeout = 40000;

                Stream readStream = myRequest.GetResponse().GetResponseStream(); // 向服务器请求,获得服务器的回应数据流


                byte[] btArray = new byte[512]; // 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length); // 向远程文件读第一次

                long currPostion = startPosition;

                while (contentSize > 0) // 如果读取长度大于零则继续读
                {
                    currPostion += contentSize;
                    int percent = (int) (currPostion * 100 / remoteFileLength);
                    //System.Console.Title="downloading percent=" + percent + "%";

                    writeStream.Write(btArray, 0, contentSize); // 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length); // 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                flag = true; //返回true下载成功


                //判断本地的文件大小
                if (File.Exists(localFile))
                {
                    var file = new FileInfo(localFile);
                    if (file.Length < 1)
                    {
                        flag = false;
                    }
                }

            }
            catch (Exception e)
            {
                using (StreamWriter writer = new StreamWriter("download-log.txt", true, Encoding.UTF8))
                {
                    writer.WriteLine(e.Message);
                    writer.WriteLine(e.StackTrace);
                }
                
                flag = false; //返回false下载失败
            }
            finally
            {
                writeStream.Close();
            }

           

            return flag;
        }

        // 从文件头得到远程文件的长度
        public static long GetHttpLength(string url)
        {
            long length = 0;

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    length = rsp.ContentLength;// 从文件头得到远程文件的长度
                }

                rsp.Close();
                return length;
            }
            catch (Exception)
            {
                return length;
            }

        }
    }
}