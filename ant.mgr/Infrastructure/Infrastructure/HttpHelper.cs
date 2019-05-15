using Infrastructure.StaticExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class HttpHelper
    {

#pragma warning disable 649
        private static readonly string ProxyUrl;
#pragma warning restore 649
        static HttpHelper()
        {
            //var proxy = ConfigHelper.GetConfig("ProxyUrl", string.Empty);
            //if (!string.IsNullOrEmpty(proxy))
            //{
            //    var port = ConfigHelper.GetConfig("ProxyPort", 8080);
            //    ProxyUrl = proxy + ":" + port;
            //}
            //else
            //{
            //    LogHelper.Warn("static WEIXIN HttpHelper()", "ProxyUrl is null");
            //    ProxyUrl = null;
            //}
        }
        public static async Task<string> GetAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                WebProxy webProxy = new WebProxy(ProxyUrl);
                request.Proxy = (IWebProxy)webProxy;
            }
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public static async Task<string> PostAsync(string url, byte[] postData, string contentType = "application/x-www-form-urlencoded")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                WebProxy webProxy = new WebProxy(ProxyUrl);
                request.Proxy = (IWebProxy)webProxy;
            }
            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = 10 * 1000;
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(postData, 0, postData.Length);
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        public static byte[] Get(string url)
        {
            WebHeaderCollection responseHeaders;
            return Get(url, out responseHeaders);
        }

        public static string GetString(string url)
        {
            return Encoding.UTF8.GetString(Get(url));
        }

        public static byte[] Get(string url, out WebHeaderCollection responseHeaders)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                WebProxy webProxy = new WebProxy(ProxyUrl);
                request.Proxy = (IWebProxy)webProxy;
            }

            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }

            try
            {
                using (var responseStream = res.GetResponseStream())
                {
                    var bytes = responseStream.ReadBytes();
                    responseHeaders = res.Headers;
                    res.Close();
                    res.Dispose();
                    return bytes;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(url);
                responseHeaders = null;
                return new byte[0];
            }
        }

        public static byte[] Post(string url, byte[] postData, string contentType = "application/x-www-form-urlencoded")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                WebProxy webProxy = new WebProxy(ProxyUrl);
                request.Proxy = (IWebProxy)webProxy;
            }
            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = 10 * 1000;//10秒



            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
                HttpWebResponse res;
                try
                {
                    res = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    res = (HttpWebResponse)ex.Response;
                }


                using (var responseStream = res.GetResponseStream())
                {
                    var bytes = responseStream.ReadBytes();
                    res.Close();
                    res.Dispose();
                    return bytes;
                }
            }
        }
    }
}
