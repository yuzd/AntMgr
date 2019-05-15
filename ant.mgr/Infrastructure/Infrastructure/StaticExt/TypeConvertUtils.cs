using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 类型转换相关工具类
    /// </summary>
    /// <remarks>yuzd 2011-11-02</remarks>
    public class TypeConvertUtils
    {
        /// <summary>
        /// 通用数据类型转换方法（泛型方法实现的类型转换）
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="value">传入值</param>
        /// <param name="defaultValue">转换失败之后，返回该默认值</param>
        /// <returns>返回类型转换成功之后的值，若转换失败则返回defaultValue</returns>
        /// <remarks>Dennis Feng by 2014-05-24</remarks>
        public static T Parse<T>(object value, T defaultValue)
        {
            T result = defaultValue; ////default(T);
            try
            {
                if (null == value)
                {
                    result = defaultValue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        result = (T)Convert.ChangeType(value, typeof(T));
                    }
                    else
                    {
                        result = defaultValue;
                    }
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }

        public static object Parse(object value, Type propType)
        {
            try
            {
                if (null == value)
                {
                    return null;
                }
                else
                {
                    if (propType.Name.Contains("Boolean"))
                    {
                        if (value.ToString().Equals("0"))
                        {
                            return false;
                        }
                        else if (value.ToString().Equals("1"))
                        {
                            return true;
                        }
                        return Convert.ChangeType(value, propType);
                    }
                    else if (propType.Name.Contains("DateTime"))
                    {
                        var t = (DateTime)Convert.ChangeType(value, propType);
                        return t;
                    }
                    else if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        return Convert.ChangeType(value, propType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
            {
                return TypeConvertUtils.StrToBool(expression, defValue);
            }
            return defValue;
        }
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(expression, "false", true) == 0)
                {
                    return false;
                }
            }
            return defValue;
        }
        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object expression, int defValue)
        {
            return TypeConvertUtils.ObjectToInt(expression, defValue);
        }
        /// <summary>
        /// 将字符串对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str, int defValue)
        {
            //if (str == null)
            //    return defValue;
            //if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*$"))
            //{
            //    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
            //    {
            //        return Convert.ToInt32(str);
            //    }
            //}
            //return defValue;      
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                return defValue;
            }
            int rv;
            if (Int32.TryParse(str, out rv))
            {
                return rv;
            }
            return Convert.ToInt32(StrToFloat(str, defValue));
        }
        /// <summary>
        /// Object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            return TypeConvertUtils.StrToFloat(strValue, defValue);
        }
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
            {
                return defValue;
            }
            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                {
                    float.TryParse(strValue, out intValue);
                }
            }
            return intValue;
        }
        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression)
        {
            return ObjectToInt(expression, 0);
        }
        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression, int defValue)
        {
            if (expression != null)
            {
                return TypeConvertUtils.StrToInt(expression.ToString(), defValue);
            }
            return defValue;
        }
        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str)
        {
            return TypeConvertUtils.StrToInt(str, 0);
        }
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
            {
                return defValue;
            }
            return TypeConvertUtils.StrToFloat(strValue.ToString(), defValue);
        }
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue)
        {
            return ObjectToFloat(strValue.ToString(), 0);
        }
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue)
        {
            if ((string.IsNullOrEmpty(strValue)))
            {
                return 0;
            }
            return TypeConvertUtils.StrToFloat(strValue.ToString(), 0);
        }
        /// <summary>
        /// double类型格式化
        /// </summary>
        /// <param name="digit">保留小数位数</param>
        /// <returns></returns>
        /// <remarks>Ralf 2012-12-22</remarks>
        public static string DoubleFormat(double d, int digit)
        {
            /*
             * 
            string money="2.23532";
            string price = Convert.ToDouble(priceStr).ToString("f0");   //小数点后一位也不保留
            Double.Parse(money).ToString("F2"); //Fn保留几位
            如果小数点后的第三位为5，则进行四舍五入，输出的小数值为2.24。
            如果money的值为2.2，则输出2.20。
             */
            return d.ToString("F" + digit + "");
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                {
                    return dateTime;
                }
            }
            return defValue;
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Now);
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }
        /// <summary>
        /// 将6位或8位int数字转换成yyyy-MM-dd (e.g. 20120706/201276 转换成 2012-07-06)
        /// </summary>
        /// <param name="date">int时间戳</param>
        /// <param name="defValue">若转换失败时，赋予默认值</param>
        /// <returns></returns>
        /// <remarks>Ralf 2012-07-06</remarks>
        public static DateTime int6Or8ToDateTime(int date, DateTime defValue)
        {
            try
            {
                DateTime dtime = DateTime.ParseExact(date.ToString(), "yyyyMMdd", null);
                return Convert.ToDateTime(dtime.ToString("yyyy-MM-dd"));
            }
            catch
            {
                return defValue;
            }
        }


        /// <summary>
        /// int型转换为string型
        /// </summary>
        /// <returns>转换后的string类型结果</returns>
        public static string IntToStr(int intValue)
        {
            return Convert.ToString(intValue);
        }

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="sbbcase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string sbbcase)
        {
            char[] c = sbbcase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }
        /// <summary>
        /// 将8位日期型整型数据转换为日期字符串数据
        /// </summary>
        /// <param name="date">整型日期</param>
        /// <param name="chnType">是否以中文年月日输出</param>
        /// <returns></returns>
        public static string FormatDate(int date, bool chnType)
        {
            string dateStr = date.ToString();
            if (date <= 0 || dateStr.Length != 8)
            {
                return dateStr;
            }
            if (chnType)
            {
                return dateStr.Substring(0, 4) + "年" + dateStr.Substring(4, 2) + "月" + dateStr.Substring(6) + "日";
            }
            return dateStr.Substring(0, 4) + "-" + dateStr.Substring(4, 2) + "-" + dateStr.Substring(6);
        }
        public static string FormatDate(int date)
        {
            return FormatDate(date, false);
        }
        /// <summary>
        /// 将字符串对象转换为long类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long StrToLong(string str, long defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 64 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                return defValue;
            }
            long rv;
            if (long.TryParse(str, out rv))
            {
                return rv;
            }
            return Convert.ToInt64(StrToFloat(str, defValue));
        }
        /// <summary>
        /// 将字符串转化成日期，失败则返回Datetime最小值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime StrToDate(string str, DateTime defValue)
        {
            try
            {
                return DateTime.Parse(str);
            }
            catch
            {
                if (defValue != null)
                {
                    return defValue;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        /// <summary>
        /// 将普通文本转换成Base64编码的文本
        /// </summary>
        /// <param name="str">文本内容</param>
        /// <returns></returns>
        /// <remarks>yuzd 2011-07-27 14:33:49</remarks>
        public static string StringToBase64String(string str)
        {
            byte[] bArr = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bArr);
        }
        /// <summary>
        /// 将Base64编码的文本转换成普通文本
        /// </summary>
        /// <param name="base64">Base64编码的文本</param>
        /// <returns></returns>
        /// <remarks>yuzd 2011-07-27 14:33:57</remarks>
        public static string Base64StringToString(string base64)
        {
            byte[] bArr = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bArr);
        }

        #region JSON
        /// <summary>
        /// 将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt">要转换的数据表</param>
        /// <returns></returns>
        public static StringBuilder DataTableToJSON(System.Data.DataTable dt)
        {
            return DataTableToJson(dt, true);
        }
        /// <summary>
        /// 将数据表转换成JSON类型串
        /// </summary>
        /// <param name="dt">要转换的数据表</param>
        /// <param name="dispose">数据表转换结束后是否dispose掉</param>
        /// <returns></returns>
        public static StringBuilder DataTableToJson(System.Data.DataTable dt, bool dtDispose)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[\r\n");
            //数据表字段名和类型数组
            string[] dt_field = new string[dt.Columns.Count];
            int i = 0;
            string formatStr = "{{";
            string fieldtype = "";
            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                dt_field[i] = dc.Caption.ToLower().Trim();
                formatStr += "'" + dc.Caption.ToLower().Trim() + "':";
                fieldtype = dc.DataType.ToString().Trim().ToLower();
                if (fieldtype.IndexOf("int") > 0 || fieldtype.IndexOf("deci") > 0 || fieldtype.IndexOf("floa") > 0 || fieldtype.IndexOf("doub") > 0 || fieldtype.IndexOf("bool") > 0)
                {
                    formatStr += "{" + i + "}";
                }
                else
                {
                    formatStr += "'{" + i + "}'";
                }
                formatStr += ",";
                i++;
            }
            if (formatStr.EndsWith(","))
            {
                formatStr = formatStr.Substring(0, formatStr.Length - 1);//去掉尾部","号
            }
            formatStr += "}},";
            i = 0;
            object[] objectArray = new object[dt_field.Length];
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                foreach (string fieldname in dt_field)
                {   //对 \ , ' 符号进行转换 
                    objectArray[i] = dr[dt_field[i]].ToString().Trim().Replace("\\", "\\\\").Replace("'", "\\'");
                    switch (objectArray[i].ToString())
                    {
                        case "True":
                            {
                                objectArray[i] = "true"; break;
                            }
                        case "False":
                            {
                                objectArray[i] = "false"; break;
                            }
                        default: break;
                    }
                    i++;
                }
                i = 0;
                stringBuilder.Append(string.Format(formatStr, objectArray));
            }
            if (stringBuilder.ToString().EndsWith(","))
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);//去掉尾部","号
            }
            if (dtDispose)
            {
                dt.Dispose();
            }
            return stringBuilder.Append("\r\n];");
        }
        #endregion
        #region 字节，字节流的转换
        /// <summary>
        /// 将Byte字节数组保存成文件
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="b"></param>
        public static void WriteAllBytes(string fullFileName, byte[] b)
        {
            File.WriteAllBytes(fullFileName, b);
        }
        /// <summary>
        /// StreamToBytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        /// <summary>
        /// StreamToFile
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public static void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[]
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 从文件读取 Stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream FileToStream(string fileName)
        {
            // 打开文件
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // 读取文件的 byte[]
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                // 把 byte[] 转换成 Stream
                Stream stream = new MemoryStream(bytes);
                return stream;
            }
        }
        #endregion
        #region 进制转换
        /// <summary>
        /// 十进制转二进制
        /// </summary>
        /// <param name="value">十进制值</param>
        /// <returns></returns>
        public static string ConvertDecimalToBinary(int value)
        {
            return Convert.ToString(value, 2).ToString();
        }
        /// <summary>
        /// 二进制转十进制
        /// </summary>
        /// <param name="value">二进制字符串</param>
        /// <returns></returns>
        public static int ConvertBinaryToDecimal(string value)
        {
            return Convert.ToInt32(value, 2);
        }
        /// <summary>
        /// 十进制转八进制
        /// </summary>
        /// <param name="value">十进制值</param>
        /// <returns></returns>
        public static string ConvertDecimalToOctal(int value)
        {
            return Convert.ToString(value, 8).ToString();
        }
        /// <summary>
        /// 八进制转十进制
        /// </summary>
        /// <param name="value">八进制字符串</param>
        /// <returns></returns>
        public static int ConvertOctalToDecimal(string value)
        {
            return Convert.ToInt32(value, 8);
        }
        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <param name="value">十进制值</param>
        /// <returns></returns>
        public static string ConvertDecimalToHex(int value)
        {
            return Convert.ToString(value, 16).ToString();
        }
        /// <summary>
        /// 十六进制转十进制
        /// </summary>
        /// <param name="value">十六进制字符串</param>
        /// <returns></returns>
        public static int ConvertHexToDecimal(string value)
        {
            return Convert.ToInt32(value, 16);
        }
        /// <summary>
        /// Byte 数组值转16进制字符串
        /// </summary>
        /// <param name="b">输入参数：Byte value</param>
        /// <returns>16进制字符串</returns>
        public static String ByteArrayToHexString(Byte[] bytes)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                temp.Append(bytes[i].ToString("X2"));
            }
            return temp.ToString();
        }
        /// <summary>
        /// Byte 值转16进制字符串
        /// </summary>
        /// <param name="b">输入参数：Byte value</param>
        /// <returns>16进制字符串</returns>
        public static String ByteToHexString(Byte b)
        {
            return b.ToString("X2");
        }
        /// <summary>
        /// 16进制字符串转 Byte 值
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #endregion
        /// <summary>  
        /// IEnumerable转换为一个DataTable  
        /// </summary>  
        /// <typeparam name="TResult"></typeparam>  
        ///// <param name="value"></param>  
        /// <returns></returns>  
        public static DataTable ToDataTable<TResult>(IEnumerable<TResult> value) where TResult : class
        {
            //创建属性的集合  
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口  
            Type type = typeof(TResult);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列  
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in value)
            {
                //创建一个DataRow实例  
                DataRow row = dt.NewRow();
                //给row 赋值  
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable  
                dt.Rows.Add(row);
            }
            return dt;
        }
        /// <summary>
        /// long转int
        /// </summary>
        /// <param name="value"></param>
        /// <returns>下溢出取Int32.MinValue，上溢出取Int32.MaxValue</returns>
        /// <reremarks>高效益</reremarks>
        public static int LongToInt(long value)
        {
            int result;
            try
            {
                result = checked((int)value);
            }
            catch
            {
                if (value > Int32.MaxValue)
                {
                    result = Int32.MaxValue;
                }
                else
                {
                    result = Int32.MinValue;
                }
            }
            return result;
        }
        /// <summary>
        /// 将List int转换成string (eg:1,2,3)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks>Dennis Feng by 2014-04-23</remarks>
        public static string ListIntToString(List<int> input)
        {
            if (null == input)
            {
                return string.Empty;
            }
            StringBuilder sbList = new StringBuilder();
            for (int i = 0; i < input.Count; i++)
            {
                sbList.Append(input[i].ToString());
                if (i < input.Count - 1)
                {
                    sbList.Append(",");
                }
            }
            return sbList.ToString();
        }

        /// <summary>
        /// 将List string 转换成string (eg: '1','2','3')
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <remarks>Dennis Feng by 2014-04-23</remarks>
        public static string ListStringToString(List<string> input)
        {
            if (null == input)
            {
                return string.Empty;
            }
            StringBuilder sbList = new StringBuilder();
            for (int i = 0; i < input.Count; i++)
            {
                sbList.Append(string.Format("{0}{1}{2}", "'", input[i].ToString(), "'"));
                if (i < input.Count - 1)
                {
                    sbList.Append(",");
                }
            }
            return sbList.ToString();
        }
        // <summary>
        /// 将十进制数转换成任意进制，支持小数
        /// </summary>
        /// <param name="value">将要转换的数</param>
        /// <param name="type">转到的进制类型，例如：36</param>
        /// <returns>字符串的结果</returns>
        public static string DToAny(double value, int type)
        {
            string H = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            long d;
            double b;
            string tempD = "", tempB = "";
            d = (long)value;
            b = value - d;
            if (d == 0)
            {
                tempD = "0";
            }
            while (d != 0)
            {
                tempD = H[(((int)d % type))] + tempD;
                d = d / type;
            }
            for (int i = 0; i < 7; i++)
            {
                if (b == 0)
                {
                    break;
                }
                tempB += H[((int)(b * type))];
                b = b * type - (int)(b * type);
            }
            if (tempB == "")
            {
                return tempD;
            }
            else
            {
                return tempD + "." + tempB;
            }
        }

    }
}
