//-----------------------------------------------------------------------
// <copyright file="Token.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


namespace ServicesModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    
    public class Token
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 登录Id
        /// </summary>
        public string Eid { get; set; }

        public string MenuRights { get; set; }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public long RoleTid { get; set; }

        public string RoleName { get; set; }
        public bool ReceiveOrderIm { get; set; }//是否接受订单推送消息
        public Token()
        {
            
        }
        public Token(string cookie)
        {
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(cookie);
            if (token == null)
            {
                throw new Exception("token invaild");
            }
            this.Eid = token.Eid;
            this.MenuRights = token.MenuRights;
            this.Code = token.Code;
            this.RoleTid = token.RoleTid;
            this.RoleName = token.RoleName;
            this.ReceiveOrderIm = token.ReceiveOrderIm;
        }
    }
}