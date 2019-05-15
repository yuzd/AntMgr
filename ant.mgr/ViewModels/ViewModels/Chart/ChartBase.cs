//-----------------------------------------------------------------------
// <copyright file="ChartBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 图表 查询基类
    /// </summary>
    public class ChartBase
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}