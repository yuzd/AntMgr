using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{

    public class CommonUtils
    {


        /// <summary>
        /// 生成指定位数的随机数
        /// </summary>
        /// <param name="length">位数</param>
        /// <returns>返回指定长度的随机数</returns>
        /// <remarks>yuzd 2011-07-27 14:40:42</remarks>
        public static string GetRandomNumber(int length)
        {
            Random randIndex = new Random();
            char[] arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour +
                                    DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond +
                                    randIndex.Next(int.MinValue, int.MaxValue));
            for (int i = 0; i < length; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }

        public static long Get10RandomInt()
        {
            var code = Guid.NewGuid().GetHashCode();
            long c = Int2Long(code);
            if (c > 100000 && c < 10000000000)
            {
                return c;
            }
            return 0;
        }

        public static long Int2Long(int i)
        {
            return (i < 0) ? 0xFFFFFFFFL + (i + 1) : i;
        }

        public static int Long2Int(long i)
        {
            return (int)(i & 0xFFFFFFFF);
        }

        #region 获取月销量的公式

        /// <summary>
        /// 获取月销量的公式
        /// </summary>
        /// <param name="mSaleNum">月真实销量</param>
        /// <param name="activityIDL">产品Id</param>
        /// <param name="commentTotalL">点评数量</param>
        /// <returns></returns>
        public static int GetMonthSaleNumByMath(long mSaleNum, long activityIDL, long commentTotalL)
        {
            int result = 0;
            double logBase = 1.05;
            double eBase = 1.075;
            int activityID = Long2Int(activityIDL);
            int commentTotal = Long2Int(commentTotalL);
            if (mSaleNum == 0)
            {
                //Round(Rand(seed)*0.7) + Ra
                result = (int)Math.Round(getRandomN(activityID, 1) * 0.7) + commentTotal;
            }
            else
            {
                //Round((R + Log(R,l))^e + Rand(seed)[0, (R+1 + Log(R+1,l))^e - (R + Log(R,l))^e))+Ra
                result = (int)Math.Round(powFunc(mSaleNum, logBase, eBase) +
                                          getRandomN(activityID,
                                              (int)Math.Round(powFunc(mSaleNum + 1, logBase, eBase) -
                                                               powFunc(mSaleNum, logBase, eBase)))) + commentTotal;
            }
            return result;
        }

        private static double getRandomN(int seed, int end_s)
        {
            double result;
            Random r = new Random(seed);
            //取数逻辑为，首先取0-1之间的一个随机数
            result = r.NextDouble();
            //然后放大到ends
            result = result * end_s;
            return result;
        }

        private static double powFunc(double mNum, double logBase, double powBase)
        {
            return Math.Pow(mNum + Math.Log(mNum, logBase), powBase);
        }

        #endregion


        #region Mask

        public static string MaskMobile(string value)
        {
            value = value.NotNull("value");
            if (value.Length > 11)
                return MaskRange(value, value.Length - 11, 11);
            return MaskRange(value, 0, value.Length);
        }

        public static string MaskLandline(string value)
        {
            value = value.NotNull("value");
            MatchCollection matchCollection = Regex.Matches(value, "\\d+", RegexOptions.Compiled);
            if (matchCollection.Count == 0)
                return value;
            List<Match> matchList = new List<Match>();
            int num1 = 0;
            int num2 = 0;
            foreach (Match match in matchCollection)
            {
                if (match.Length > num1)
                {
                    matchList.Add(match);
                    num1 = match.Length;
                    num2 = 1;
                }
                else if (match.Length == num1)
                {
                    matchList.Add(match);
                    ++num2;
                }
            }
            Match match1 = matchList[matchList.Count - num2 + num2 / 2];
            return MaskRange(value, match1.Index, match1.Length);
        }

        public static string MaskEmail(string value)
        {
            value = value.NotNull("value");
            int rangeLength = value.IndexOf('@');
            if (rangeLength < 0)
                return value;
            return MaskRange(value, 0, rangeLength);
        }

        public static string MaskIDNumber(string value)
        {
            value = value.NotNull("value");
            if (value.Length > 3)
                return DoMask(value, value.Length / 3, value.Length - value.Length / 3 - 2);
            return MaskRange(value, 0, value.Length);
        }

        public static string MaskBankCard(string value)
        {
            value = value.NotNull("value");
            if (value.Length > 10)
                return DoMask(value, 6, value.Length - 10);
            return MaskRange(value, 0, value.Length);
        }

        public static string MaskAddress(string value)
        {
            value = value.NotNull("value");
            int rangeStart = value.Length / 2;
            int num1 = value.IndexOf('区');
            if (num1 > -1)
            {
                if (num1 < rangeStart)
                    return MaskRange(value, num1 + 1, value.Length - num1 - 1);
            }
            else
            {
                int num2 = value.IndexOf('市');
                if (num2 > -1)
                {
                    if (num2 < rangeStart)
                        return MaskRange(value, num2 + 1, value.Length - num2 - 1);
                }
                else
                {
                    int num3 = value.IndexOf('省');
                    if (num3 > -1 && num3 < rangeStart)
                        return MaskRange(value, num3 + 1, value.Length - num3 - 1);
                }
            }
            return MaskRange(value, rangeStart, value.Length - rangeStart);
        }

        public static string MaskCommon(string value)
        {
            value = value.NotNull("value");
            return MaskRange(value, 0, value.Length);
        }

        public static string MaskRange(string value, int rangeStart, int rangeLength)
        {
            if (value.Length < 3 || rangeLength < 3)
                return value;
            int maskStart = rangeStart + rangeLength / 3;
            int maskLength = rangeLength / 3 + 1;
            return DoMask(value, maskStart, maskLength);
        }

        public static string DoMask(string value, int maskStart, int maskLength)
        {
            char[] charArray = value.ToCharArray();
            int index1 = maskStart;
            for (int index2 = maskStart + maskLength - 1; index1 <= index2; ++index1)
                charArray[index1] = '*';
            return new string(charArray);
        }

        #endregion

        #region 根据身份证号获取性别 年纪

        public static async Task<BirthdayAgeSex> GetBirthdayAgeSexAsync(string identityCard)
        {
            if (string.IsNullOrEmpty(identityCard))
            {
                return null;
            }
            return await Async.AsyncHelper.GetTask<BirthdayAgeSex>(() => GetBirthdayAgeSex(identityCard),
                CancellationToken.None);
        }

        /// <summary>
        /// 根据身份证号获取性别 年纪
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public static BirthdayAgeSex GetBirthdayAgeSex(string identityCard)
        {
            if (string.IsNullOrEmpty(identityCard))
            {
                return null;
            }
            else
            {
                if (identityCard.Length != 15 && identityCard.Length != 18) //身份证号码只能为15位或18位其它不合法
                {
                    return null;
                }
            }

            if (!IsIDCard(identityCard))
            {
                return null;
            }

            BirthdayAgeSex entity = new BirthdayAgeSex();
            string strSex = string.Empty;
            if (identityCard.Length == 18) //处理18位的身份证号码从号码中得到生日和性别代码
            {
                entity.Birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" +
                                  identityCard.Substring(12, 2);
                strSex = identityCard.Substring(14, 3);
            }
            if (identityCard.Length == 15)
            {
                entity.Birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" +
                                  identityCard.Substring(10, 2);
                strSex = identityCard.Substring(12, 3);
            }

            entity.Age = CalculateAge(entity.Birthday); //根据生日计算年龄
            if (int.Parse(strSex) % 2 == 0) //性别代码为偶数是女性奇数为男性
            {
                entity.Sex = "女";
            }
            else
            {
                entity.Sex = "男";
            }
            return entity;
        }

        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDate">生日</param>
        /// <returns></returns>
        public static int CalculateAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month ||
                (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
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
                if (long.TryParse(id.Remove(17), out n) == false || n < Math.Pow(10, 16) ||
                    long.TryParse(id.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false; //数字验证
                }
                string address =
                    "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(id.Remove(2)) == -1)
                {
                    return false; //省份验证
                }
                string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false; //生日验证
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
                    return false; //校验码验证
                }
                return true; //符合GB11643-1999标准
            }
            else
            {
                return false; //位数不对
            }
        }

        #endregion

        /// <summary>
        /// 是否是正常的邮箱
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmil(string str)
        {
            Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            return r.IsMatch(str);
        }
    }
}

/// <summary>
/// 定义 生日年龄性别 实体
/// </summary>
public class BirthdayAgeSex
{
    public string Birthday { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
}

public class UploadWsFileResult
{
    public bool IsSuccess { get; set; }
    public string Path { get; set; }
}
