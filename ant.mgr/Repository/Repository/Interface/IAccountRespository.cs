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
    /// 系统用户
    /// </summary>
    public interface IAccountRespository : IRepository<SystemUsers>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> LogOn(LogOnVM info);
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Tuple<long, List<UserSM>>> GetUserList(AccountVm model,Token user);
        /// <summary>
        /// 给用户赋予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserAddRole(UserAddRoleVm info);
        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="userTid"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserDelete(long userTid);
        /// <summary>
        /// 更改用户的属性
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> ChangeField(ChangeFieldVm info);
        /// <summary>
        /// 手动添加用户
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserAdd(SystemUsers info,Token user);
    }
}