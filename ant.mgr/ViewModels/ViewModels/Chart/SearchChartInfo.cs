//-----------------------------------------------------------------------
// <copyright file="SearchChartInfo.cs" company="Company">
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
    /// 图表 查询  直接包装成对象ConditionModel
    /// </summary>
    public class SearchChartInfo<T> :ChartBase
    {
        private T data;
        public T Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}