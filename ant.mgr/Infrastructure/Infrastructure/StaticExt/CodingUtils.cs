//-----------------------------------------------------------------------
// <copyright file="CodingUtils.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using Configuration;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace Infrastructure.StaticExt
{

    public class CodingUtils
    {
        #region

        /// <summary>
        /// 对字符串进行自定义格式加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncryptCarKeys(string source)
        {
            string key0 = "CloudBag"; //加点盐
            source = string.Format("{0}{1}", source, key0);
            return SHA256(source);
        }

        #endregion

        #region SHA1

        public static string GetSwcSH1(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sh1 = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sh1.Append(data[i].ToString("x2").ToUpperInvariant());
            }
            return sh1.ToString();
        }

        #endregion

        #region SHA256

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        #endregion

        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string SHA512(string source)
        {
            string result = string.Empty;
            SHA512 sha512 = new SHA512Managed();
            byte[] s = sha512.ComputeHash(Encoding.UTF8.GetBytes(source));
            for (int i = 0; i < s.Length; i++)
            {
                result += s[i].ToString("X");
            }
            sha512.Clear();
            return result;
        }

        /// <summary>
        /// 字符串md5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StrToMd5(string input)
        {
            var base64 = input.ToBase64();
            var md5 = MD5(base64);
            return md5;
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        #region AES

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string AesEncrypt(string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt))
            {
                return string.Empty;
            }
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(@"F30F6FD087514424B671C397AF1C1C51");

                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string AesDecrypt(string toDecrypt)
        {
            if (string.IsNullOrEmpty(toDecrypt))
            {
                return string.Empty;
            }
            try
            {

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(@"F30F6FD087514424B671C397AF1C1C51");

                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region JWT

        /// <summary>
        /// Json Web Token 加密
        /// </summary>
        /// <param name="str">加密字符串</param>
        /// <param name="expSeconds">失效时间(默认30秒)</param>
        /// <param name="noExp">不设置失效</param>
        /// <returns></returns>
        public static string JwTEncode<T>(T str, int expSeconds = 30, bool noExp = false)
        {
            JwtBuilder builder = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(GlobalSetting.JWTSecretKey)
               .AddClaim("data", str);

            if (!noExp)
            {
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds) + expSeconds;
                builder = builder.AddClaim("exp", now);
            }
            string token = builder.Build();
            return token;
        }

        /// <summary>
        /// Json Web Token 解密
        /// </summary>
        /// <typeparam name="T">获取的类型</typeparam>
        /// <param name="token">加密JWT</param>
        /// <returns></returns>
        public static T JwTDecode<T>(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var payload =
                    decoder.DecodeToObject<IDictionary<string, object>>(token, GlobalSetting.JWTSecretKey,
                        verify: true);
                if (payload == null)
                {
                    return default(T);
                }

                var data = (T)payload["data"];
                return data;
            }
            catch (TokenExpiredException) //过期
            {
                return default(T);
            }
            catch (SignatureVerificationException)
            {
                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        #endregion

        #region JavaScriptDateConverter
        public static long ConvertTojs(DateTime TheDate)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = TheDate.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }
        public static DateTime ConvertFromJs(long milliTime)
        {
            try
            {
                long timeTricks = new DateTime(1970, 1, 1).Ticks + milliTime * 10000 + TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
                return new DateTime(timeTricks);
            }
            catch (Exception)
            {

                return DateTime.Now;
            }
        }
        #endregion

        /// <summary>
        /// 解密对应js的 AtoB
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string BtoA(string str)
        {
            int mod4 = str.Length % 4;
            if (mod4 > 0)
            {
                str += new string('=', 4 - mod4);
            }
            byte[] data = Convert.FromBase64String(str);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        /// <summary>
        /// 秒转成时分秒
        /// </summary>
        /// <param name="senconds"></param>
        /// <returns></returns>
        public static string ConvertFromSenconds(int senconds)
        {
            if (senconds <= 0)
            {
                return "0";
            }
            TimeSpan ts = new TimeSpan(0, 0, senconds);
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "分 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "秒";
            }
            return str;
        }

        //去掉字符串中的非数字
        public static string RemoveNotNumber(string key)
        {
            return Regex.Replace(key, @"[^\d]*", "");
        }

    }
}