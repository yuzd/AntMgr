//-----------------------------------------------------------------------
// <copyright file="ResultJsonBase.cs" company="Company">
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
    /// ResultJsonBase
    /// </summary>
    public class ResultJsonBase
    {

        public ResultJsonBase()
        {

        }

        #region field

        private int status;
        private string info;

        #endregion

        #region property

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态(0:失败 1:成功)")]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        [Description("提示信息")]
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        #endregion
    }
}