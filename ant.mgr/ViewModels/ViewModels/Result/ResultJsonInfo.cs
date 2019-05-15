//-----------------------------------------------------------------------
// <copyright file="ResultJsonInfo.cs" company="Company">
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
    /// API返回的Json模型（用于Select等需要返回数据集的场景）
    /// </summary>
    public class ResultJsonInfo<T> : ResultJsonBase
    {
        public ResultJsonInfo()
        {

        }

        #region field

        private T data;

        #endregion

        #region property

        /// <summary>
        /// 返回数据
        /// </summary>
        [Description("返回数据")]
        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion
    }
}