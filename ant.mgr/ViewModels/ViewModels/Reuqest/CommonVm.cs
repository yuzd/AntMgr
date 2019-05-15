using Infrastructure.StaticExt;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels.Condition;

namespace ViewModels.Reuqest
{



    public class CodeGenVm
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段列表
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (string.IsNullOrEmpty(ColumnStr))
                {
                    return new List<string>();
                }
                return ColumnStr.Split(',').ToList();
            }
        }

        /// <summary>
        /// 字段名称列表多个逗号隔开
        /// </summary>
        public string ColumnStr { get; set; }
    }

    public class BadjsVm
    {
        public string id { get; set; } //APPID
        public string uin { get; set; }
        public string msg { get; set; }
        public string target { get; set; }
        public long rowNum { get; set; }
        public long colNum { get; set; }
        public string from { get; set; }
        public int level { get; set; }
        public long _t { get; set; }

        public DateTime? clientTime
        {
            get { return CodingUtils.ConvertFromJs(_t); }
        }
    }

   
}   
