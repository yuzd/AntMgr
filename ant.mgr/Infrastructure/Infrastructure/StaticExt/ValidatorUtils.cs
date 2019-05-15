using System;
using System.Text.RegularExpressions;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 对象格式判断、验证工具类
    /// </summary>
    /// <remarks>yuzd 2011-11-03 14:33:22</remarks>
    public class ValidatorUtils
    {
        /// <summary>
        /// 是否是正整数
        /// </summary>
        /// <param name="number">数字字符串</param>
        /// <returns></returns>
        /// <remarks>Ralf 2012-06-07</remarks>
        public static bool IsPositiveInteger(string number)
        {
            try
            {
                for (int i = 0; i < number.Length; i++)
                {
                    if (!char.IsNumber(number, i))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
            {
                return IsNumeric(expression.ToString());
            }
            return false;
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
            {
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            }
            return false;
        }
        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            if (strNumber.Length < 1)
            {
                return false;
            }
            foreach (string id in strNumber)
            {
                if (!IsNumeric(id))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }
        public static bool IsValidDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        /// <summary>
        /// 是否是中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese(string str)
        {
            return Regex.IsMatch(str, @"^[\u4e00-\u9fa5]+$");
        }
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        /// <summary>
        /// 判断字符串是否是合法的GUID
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        /// <remarks>yuzd 2011-07-27 14:33:36</remarks>
        public static bool IsGuid(string str)
        {
            Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(str.Trim());
        }
        /// <summary>
        /// 判断字符串是否是合法的GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsGuidByReg(string strSrc)
        {
            Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled);
            return reg.IsMatch(strSrc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strToValidate"></param>
        /// <returns></returns>
        public bool IsGuid2(string strToValidate)
        {
            bool isGuid = false;
            string strRegexPatten = @"^(/{){0,1}[0-9a-fA-F]{8}/-[0-9a-fA-F]{4}/"
                    + @"-[0-9a-fA-F]{4}/-[0-9a-fA-F]{4}/-[0-9a-fA-F]{12}(/}){0,1}$";
            if (strToValidate != null && !strToValidate.Equals(""))
            {
                isGuid = System.Text.RegularExpressions.Regex.IsMatch(strToValidate, strRegexPatten);
            }
            return isGuid;
        }
        /// <summary>
        /// 判断字符串是否是合法的GUID (需要.NET4.0以及以上版本才支持)
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsGuidByParse(string strSrc)
        {
            Guid g = Guid.Empty;
            return Guid.TryParse(strSrc, out g);
        }
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns>是否是合法的手机号码</returns>
        /// <remarks>yuzd 2011-07-27 14:33:42</remarks>
        public static bool IsMobilePhone(string mobile)
        {
            return Regex.IsMatch(mobile, @"^1[3458]\d{9}$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 是否为IPV4地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPv4(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 是否为IPv6地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <remarks>Ralf 2012-06-07</remarks>
        public static bool IsIPv6(string ip)
        {
            return Regex.IsMatch(ip, @"^([\da-fA-F]{1,4}:){6}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^::([\da-fA-F]{1,4}:){0,4}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:):([\da-fA-F]{1,4}:){0,3}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){2}:([\da-fA-F]{1,4}:){0,2}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){3}:([\da-fA-F]{1,4}:){0,1}((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){4}:((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$|^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$ ");
        }
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }
        /// <summary>
        /// 是否是时间格式
        /// </summary>
        /// <param name="timeval"></param>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }
        /// <summary>
        /// 是否是日期时间型
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsDateTime(dynamic d)
        {
            return d is DateTime;
        }
        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        /// <summary>
        /// 验证位身份证
        /// </summary>
        /// <param name="id">身份证号码</param>
        /// <returns>是否真实身份证</returns>
        public static bool IsIDCard(string id)
        {
            int intLen = id.Length;
            long n = 0;
            if (intLen == 15)
            {
                string ts = string.Empty;
                if (Convert.ToInt32(id.Substring(6, 2)) > 20)
                {
                    ts = "19";
                }
                else
                {
                    ts = "20";
                }
                string IDCode18 = id.Substring(0, 6) + ts + id.Substring(6);
                int[] wi = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
                string[] wf = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
                int s = 0;
                for (int i = 0; i < 17; i++)
                {
                    s += wi[i] * Convert.ToInt32(IDCode18.Substring(i, 1));
                }
                IDCode18 += wf[Convert.ToInt32(s) % 11];
                id = IDCode18;
                /*
                if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
                {
                    return false;//数字验证
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(Id.Remove(2)) == -1)
                {
                    return false;//省份验证
                }
                string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
               /* double iSum = 0;
                for (int i = 17; i >= 0; i--)
                {
                    iSum += (System.Math.Pow(2, i) % 11) * int.Parse(Id[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);
                }
                if (iSum % 11 != 1)
                    return false; 
                */
                // return true;//符合15位身份证标准
            }
            if (id.Length == 18)
            {
                if (long.TryParse(id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(id.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;//数字验证
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(id.Remove(2)) == -1)
                {
                    return false;//省份验证
                }
                string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = id.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != id.Substring(17, 1).ToLower())
                {
                    return false;//校验码验证
                }
                return true;//符合GB11643-1999标准
            }
            else
            {
                return false;//位数不对
            }
        }
        /// <summary>
        /// 是否为有效域名
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            Regex r = new Regex(@"^\d+$");
            if (host.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                return false;
            }
            return !r.IsMatch(host.Replace(".", string.Empty));
        }
        /// <summary>
        /// 是否是闰年（规则：能被4整除却不能被100整除 或 能被400整除的年份是闰年）
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns></returns>
        /// <remarks>Ralf 2012-06-26</remarks>
        public static bool IsLeapyear(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
            {
                return true;
            }
            return false;
        }
    }
}
