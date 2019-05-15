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
    using System.Threading.Tasks;


    /// <summary>
    /// 
    /// </summary>
    public interface IRoleRespository : IRepository<SystemRole>
    {
        Task<Tuple<long, List<SystemRole>>> GetList(RoleVm model, Token eid);
        Task<string> AddRole(AddRoleVm role, Token uid);
        Task<List<SystemRole>> GetAllRoleList(Token uid);
        Task<SystemAction> GetSystemUserActions(string eid, long RoleTid, long currentMenuTid);
        Task<string> AddRoleActions(RoleAction model);
        Task<List<SystemPageAction>> GetRoleActions(RoleAction model);
        Task<string> DeleteRole(long tid, Token user);
    }
}