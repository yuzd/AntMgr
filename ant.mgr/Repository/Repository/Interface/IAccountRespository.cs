//-----------------------------------------------------------------------
// <copyright file="IAccount.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using DbModel;
using ServicesModel;
using ViewModels.Reuqest;

namespace Repository.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public interface IAccountRespository : IRepository<SystemUsers>
    {
        Task<Tuple<bool, string>> LogOn(LogOnVM info);
        Task<Tuple<long, List<UserSM>>> GetUserList(AccountVm model,Token user);
        Task<Tuple<bool, string>> UserAddRole(UserAddRoleVm info);
        Task<Tuple<bool, string>> UserDelete(long userTid);
        Task<Tuple<bool, string>> ChangeField(ChangeFieldVm info);
        Task<Tuple<bool, string>> UserAdd(SystemUsers info,Token user);
    }
}