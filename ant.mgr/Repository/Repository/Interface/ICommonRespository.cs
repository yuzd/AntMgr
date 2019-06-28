//-----------------------------------------------------------------------
// <copyright file="IAccount.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using DbModel;
using ServicesModel;
using ViewModels.Reuqest;

namespace Repository.Interface
{
    using System.Collections.Generic;


    /// <summary>
    /// 公共处理
    /// </summary>
    public interface ICommonRespository : IRepository
    {
        /// <summary>
        /// 获取所有的Table和Columns
        /// </summary>
        /// <returns></returns>
        string GetDbTablesAndColumns();

        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <returns></returns>
        List<CodeGenTable> GetDbTables();

        /// <summary>
        /// 获取表下面所有的字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<CodeGenField> GetDbTablesColumns(string tableName);

        /// <summary>
        /// 自动生成代码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        byte[] CodeGen(CodeGenVm model);
    }
}