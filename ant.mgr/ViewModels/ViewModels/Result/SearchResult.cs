//-----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace ViewModels.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 搜索结果
    /// </summary>
    public class SearchResult<T> : Result.ResultJsonBase
    {

       

        #region property

        #region 分页信息


        /// <summary>
        /// 总记录数
        /// </summary>
        [Description("总记录数")]
        public long Total { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [Description("总记录数")]
        public T Rows{get; set; }

        #endregion

        #endregion
    }
}