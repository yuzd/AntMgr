
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Excel
{
    /// <summary>
    /// 实体类生成特性
    /// 可以控制显示中文名
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface,
        AllowMultiple = false, Inherited = true)]
    public class ExcelFieldAttribute : Attribute
    {
        /// <summary>
        /// 显示中文名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderRule { get; set; }


        public ExcelFieldAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface,
        AllowMultiple = true, Inherited = true)]
    public class ExcelClassAttribute : Attribute
    {
        /// <summary>
        /// 显示中文名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderRule { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public string Column { get; set; }


        public ExcelClassAttribute(string name)
        {
            Name = name;
        }
    }
}
