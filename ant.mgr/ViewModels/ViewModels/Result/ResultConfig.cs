//-----------------------------------------------------------------------
// <copyright file="ResultConfig.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class ResultConfig
    {
        /// <summary>
        /// 执行成功返回值("1")
        /// </summary>
        public static int Ok = 1;
        /// <summary>
        /// 执行成功返回值提示("成功")
        /// </summary>
        public static string SuccessfulMessage = "操作成功！";


        /// <summary>
        /// 执行失败返回值("0")
        /// </summary>
        public static int Fail = 0;
        /// <summary>
        /// 执行失败返回值提示（失败）
        /// </summary>
        public static string FailMessage = "操作失败！";

        /// <summary>
        /// 
        /// </summary>
        public static int NoPower  =2;
        /// <summary>
        /// 
        /// </summary>
        public static string FailMessageForNoPower = "没有权限，操作失败！";


        public static string FailMessageForSystem = "系统出错，操作失败！";

        public static string FailMessageForNotFound = "系统出错,404-NotFound！";
    }
}