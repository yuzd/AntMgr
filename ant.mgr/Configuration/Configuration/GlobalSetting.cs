using System;
using System.Collections.Generic;
using System.Linq;

namespace Configuration
{
    public class GlobalSetting
    {

        public static string CurrentRootPath;

        /// <summary>
        /// JWT 私有秘钥
        /// </summary>
        public const string JWTSecretKey = "GQDstcKxs0NHjPOuXOYg5MbeJ1XT0uFiwDV8VBr8";

        /// <summary>
        /// 当前登陆用户的Eid
        /// </summary>
        public static string CurrentLoginUserGuid = "cbeid";
        /// <summary>
        /// 带有权限的MenuTid
        /// </summary>
        public static string CurrentMenu = "cbmenu";


        #region 上帝模式

        public static List<string> GoldList
        {
            get
            {
                var result = new List<string>();
                var list = ConfigHelper.GetConfig("GoldList", string.Empty)
                    .Split(new string[] { "],[" }, StringSplitOptions.None).ToList();
                foreach (var li in list)
                {
                    result.Add(li.Replace("]", "").Replace("[", ""));
                }
                return result;
            }
        }
        #endregion
    }
}
