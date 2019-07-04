//-----------------------------------------------------------------------
// <copyright file="ReuqestBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace ViewModels.Reuqest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 适用于 WebAPI的请求包装
    /// </summary>
    public class ReuqestBase
    {
        public ReuqestBase()
        {
           // this.Token = string.Empty;

        }
       // public string Token { get; set; }
    }


    public class RequestInfo<T> : ReuqestBase
    {
        public RequestInfo()
        {
            this.Data = default(T);
        }

        [Description("请求参数对象")]
        public T Data { get; set; }
    }
}