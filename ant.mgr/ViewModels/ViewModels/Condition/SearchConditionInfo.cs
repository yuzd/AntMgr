//-----------------------------------------------------------------------
// <copyright file="SearchConditionInfo.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Condition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 查询 适用于分页查询 直接包装成对象ConditionModel
    /// </summary>
    public class SearchConditionInfo<T> : ConditionBase
    {
        private T conditionModel;

        public T ConditionModel
        {
            get { return conditionModel; }
            set { conditionModel = value; }
        }
    }
}