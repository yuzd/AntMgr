using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels.Reuqest
{
    public class LogOnVM
    {
       
        /// <summary>
        /// 登录名称
        /// </summary>
        public string eid { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string pwd { get; set; }

        
    }

    public class UserAddRoleVm
    {
        /// <summary>
        /// 用户Tid
        /// </summary>
        public string UserTid { get; set; }

        /// <summary>
        /// 角色Tid
        /// </summary>
        public long RoleTid { get; set; }

    }

    public class ChangeFieldVm
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Tid { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

    }
}
