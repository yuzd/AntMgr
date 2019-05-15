using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static class ObjectExtensions
    {


        /// <summary>
        /// 把元转成分
        /// </summary>
        /// <param name="yuan"></param>
        /// <returns></returns>
        public static int YuanToFen(this decimal yuan)
        {
            if (yuan <= 0) return 0;
            return (int)(100 * (yuan * 1000) / 1000);
        }

        /// <summary>
        /// 分转成元
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public static decimal FenToYuan(this int fen)
        {
            if (fen <= 0) return 0;
            return (fen / 100.0M);
        }

        /// <summary>
        /// 把Dictionary 加在string 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        public static string AppendDic(this string value, Dictionary<string, string> addition)
        {
            if (addition == null) return value;
            value += Environment.NewLine + "            [Tags]" + Environment.NewLine;
            foreach (KeyValuePair<string, string> item in addition)
            {
                value += "                 {Key:" + item.Key + ",Value:" + item.Value + "}" + Environment.NewLine;
            }
            return value;
        }

        /// <summary>
        /// 判断是否是神盾手机号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsShendunPhone(this string str) { return Regex.Matches(str, "[a-zA-Z]").Count > 0; }
        public static string DecodeFromBase64(this string base64EncodedString)
        {
            return DecodeFromBase64(base64EncodedString, Encoding.UTF8);
        }

        public static string DecodeFromBase64(this string base64EncodedString, Encoding encoding)
        {
            if (string.IsNullOrEmpty(base64EncodedString))
            {
                return base64EncodedString;
            }

            var bytes = Convert.FromBase64String(base64EncodedString);

            return encoding.GetString(bytes);
        }
        /// <summary>
        /// 获取13位的时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetMillisecondTime(this DateTime dt)
        {
            return (long)(dt - DateTime.Parse("1970-1-1")).TotalMilliseconds;
        }

        /// <summary>
        /// 将流读取成字节组。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>字节组。</returns>
        public static byte[] ReadBytes(this Stream stream)
        {
            if (!stream.NotNull("stream").CanRead)
                throw new NotSupportedException(stream + "不支持读取。");

            Action trySeekBegin = () =>
            {
                if (!stream.CanSeek)
                    return;

                stream.Seek(0, SeekOrigin.Begin);
            };

            trySeekBegin();

            var list = new List<byte>(stream.CanSeek ? (stream.Length > int.MaxValue ? int.MaxValue : (int)stream.Length) : 300);

            int b;

            while ((b = stream.ReadByte()) != -1)
                list.Add((byte)b);

            trySeekBegin();

            return list.ToArray();
        }
        /// <summary>
        /// 转变成Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToJson<T>(this string value, bool ignoreError = false)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return default(T);
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                if (!ignoreError) Logging.LogHelper.Warn("ToJson", "value:" + value + "||" + ex);
            }
            return default(T);
        }

        public static string ToJsonString(this object value)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                Logging.LogHelper.Warn("ToJsonString", "value:" + value + "||" + ex);
            }
            return "SerializeObject Error";
        }

        /// <summary>
        /// 从左边开始截取字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string format, int length)
        {
            if (string.IsNullOrEmpty(format))
            {
                return format;
            }
            if (format.Length < length)
            {
                length = format.Length;
            }
            return format.Substring(0, length);
        }

        /// <summary>
        /// 从右边开始截取字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string format, int length)
        {
            if (string.IsNullOrEmpty(format))
            {
                return format;
            }
            var allLength = format.Length;
            if (allLength < length)
            {
                length = format.Length;
            }
            return format.Substring(allLength - length, length);
        }

        public static string Args(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        /// <summary>
        /// 不允许为Null。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>对象实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> 为null。</exception>
        public static T NotNull<T>(this T instance, string name) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(name.NotEmpty("name"));
            return instance;
        }
        /// <summary>
        /// 不允许空字符串。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>字符串。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为空。</exception>
        public static string NotEmpty(this string str, string name)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(name.NotEmpty("name"));
            return str;
        }
        /// <summary>
        /// 不允许空和只包含空格的字符串。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>字符串。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为空或者全为空格。</exception>
        public static string NotEmptyOrWhiteSpace(this string str, string name)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(name.NotEmpty("name"));
            return str;
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static string ToBase64(this string str)
        {
            return ToBase64(str, Encoding.UTF8);
        }

        public static string ToBase64(this string str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return Convert.ToBase64String(encoding.GetBytes(str));
        }
        public static string DecodeBase64(this string str)
        {
            try
            {
                byte[] data = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static int GetDifferenceInDays(this DateTime startDate, DateTime endDate)
        {
            TimeSpan ts = endDate - startDate;
            int totalDays = (int)Math.Ceiling(ts.TotalDays);
            if (ts.TotalDays < 1 && ts.TotalDays > 0)
                totalDays = 0;
            else
                totalDays = (int)(ts.TotalDays);
            return totalDays;
        }

        /// <summary>
        /// 改变参数的类型
        /// </summary>
        /// <typeparam name="Type">要改成的类型</typeparam>
        /// <param name="value">要改变的对象</param>
        /// <returns>改变后的类型</returns>
        public static Type ChangeType<Type>(this object value)
        {
            try
            {
                return (Type)System.Convert.ChangeType(value, typeof(Type));
            }
            catch { return default(Type); }
        }



        /// <summary>
        /// 检查email是否合法
        /// </summary>
        /// <param name="email">要检查的email</param>
        /// <returns>bool</returns>
        public static bool CheckEmail(this string email)
        {
            var _email = null == email ? System.String.Empty : email.Trim();

            if (System.String.IsNullOrEmpty(_email))
            {
                return false;
            }

            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }



        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 对元素集合进行并发的ForEach处理，返回处理结果的集合
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <param name="asyncMethod"></param>
        /// <returns></returns>
        public static async Task<List<TOut>> ForEachAsync<TIn, TOut>(
            this IEnumerable<TIn> source, Func<TIn, Task<TOut>> asyncMethod)
        {
            List<Task<TOut>> tasks = new List<Task<TOut>>();
            ConcurrentQueue<TOut> results = new ConcurrentQueue<TOut>();
            foreach (var item in source)
            {
                Task<TOut> resp = asyncMethod(item);
                tasks.Add(resp);
            }
            var processingTasks = (from t in tasks select CollectResult(t, results)).ToArray();
            await Task.WhenAll(processingTasks);

            return results.ToList();
        }

        /// <summary>
        /// 对元素集合进行并发的ForEach处理。返回输入元素和结果的一一对应关系
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <param name="asyncMethod"></param>
        /// <returns>Item1为输入参数，Item2为结果</returns>
        public static async Task<List<Tuple<TIn, TOut>>> ForEachAsync2<TIn, TOut>(
            this IEnumerable<TIn> source, Func<TIn, Task<TOut>> asyncMethod)
        {
            List<Tuple<TIn, Task<TOut>>> tasks = new List<Tuple<TIn, Task<TOut>>>();
            ConcurrentQueue<Tuple<TIn, TOut>> results = new ConcurrentQueue<Tuple<TIn, TOut>>();
            foreach (var item in source)
            {
                Task<TOut> resp = asyncMethod(item);
                tasks.Add(new Tuple<TIn, Task<TOut>>(item, resp));
            }
            var processingTasks = (from t in tasks select CollectResult2(t.Item1, t.Item2, results)).ToArray();
            await Task.WhenAll(processingTasks);

            return results.ToList();
        }

        private static async Task CollectResult2<TIn, TOut>(
            TIn item1, Task<TOut> item2, ConcurrentQueue<Tuple<TIn, TOut>> results)
        {
            try
            {
                var r = await item2;
                results.Enqueue(new Tuple<TIn, TOut>(item1, r));
            }
            catch (Exception)
            {
                results.Enqueue(new Tuple<TIn, TOut>(item1, default(TOut)));
            }
        }

        private static async Task CollectResult<TOut>(Task<TOut> t, ConcurrentQueue<TOut> results)
        {
            try
            {
                var v = await t;
                results.Enqueue(v);
            }
            catch (Exception)
            {
                results.Enqueue(default(TOut));
            }
        }

        public static byte[] Serialize(this object obj)
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);

            return stream.ToArray();
        }

        public static T Deserialize<T>(this byte[] data) where T : class
        {
            var stream = new MemoryStream(data);
            var formatter = new BinaryFormatter();

            return formatter.Deserialize(stream) as T;
        }

        /// <summary>
        /// 快速将包含大小写的字符串转换为全大写，对小写字母转义，例如a会变成^A
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeToCaseInsensitive(this string str, bool toLower = false)
        {
            Regex reg = new Regex(toLower ? "[A-Z]" : "[a-z]");
            str = str.Replace("^", toLower ? "^~" : "^>");
            Match match = reg.Match(str);
            int index = 0;
            StringBuilder stringBuilder = new StringBuilder();
            var dalt = toLower ? 32 : -32;
            while (match.Success)
            {
                if (index < match.Index)
                {
                    stringBuilder.Append(str.Substring(index, match.Index - index));
                }

                stringBuilder.Append('^');
                stringBuilder.Append((char)(match.Value[0] + dalt));
                index = match.Index + match.Length;
                match = match.NextMatch();
            }

            if (index < str.Length - 1)
            {
                stringBuilder.Append(str.Substring(index));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// EncodeToCaseInsensitive的逆方法，还原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeToCaseSensitive(this string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var index = str.IndexOf('^');
            if (index < 0 || index == str.Length - 1)
            {
                return str;
            }

            var c = str[index + 1];
            var dalt = (c == '~' || c >= 'a' && c <= 'z') ? -32 : 32;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '^')
                {
                    i += 1;
                    if (i < str.Length)
                    {
                        stringBuilder.Append((char)(str[i] + dalt));
                    }
                }
                else
                {
                    stringBuilder.Append(str[i]);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将一个Decimal转换成整数，可以自定义舍去和进位的阈值
        /// </summary>
        /// <param name="numberToDiscard">舍去的阈值，例如四舍五入为0.5，小于0.2舍去是0.2</param>
        /// <returns></returns>
        public static int ToInt(this decimal num, decimal numberToDiscard) {
            int a = Convert.ToInt32(num);
            if ((num % 1M) >= numberToDiscard) {
                a += 1;
            }

            return a;
        }

        public static string GetFileName(this string url)
        {
            return url.Substring(url.LastIndexOfAny(new[] { '\\', '/' }) + 1);
        }
    }
}
