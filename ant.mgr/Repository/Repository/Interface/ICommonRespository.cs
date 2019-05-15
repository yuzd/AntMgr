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
    /// 
    /// </summary>
    public interface ICommonRespository : IRepository
    {

        string GetDbTablesAndColumns();
        List<CodeGenTable> GetDbTables();
        List<CodeGenField> GetDbTablesColumns(string tableName);
        byte[] CodeGen(CodeGenVm model);
    }
}