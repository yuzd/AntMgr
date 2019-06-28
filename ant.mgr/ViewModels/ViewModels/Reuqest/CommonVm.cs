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

  

   
}   
