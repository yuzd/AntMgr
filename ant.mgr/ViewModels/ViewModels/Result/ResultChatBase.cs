//-----------------------------------------------------------------------
// <copyright file="ResultChatBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public class ResultChatBase
    {
        public string Name { get; set; }
        public Decimal Value { get; set; }
        public string GroupName { get; set; }
    }
}