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
    using System.Collections.Generic;


    /// <summary>
    /// 菜单处理
    /// </summary>
    public interface IMenuRespository : IRepository<SystemMenu>
    {
        /// <summary>
        /// 禁用某菜单
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        string DisableMenu(long menuTid);
        /// <summary>
        /// 加载menuTree
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        SystemMenuSM GetCurrentMenu(long menuTid);
        /// <summary>
        /// 更新某个菜单的信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string UpdateMenu(AddMenuVm model);
        /// <summary>
        /// 新增Menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMenu(AddMenuVm model);
        /// <summary>
        /// 获取子菜单
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetSubMenus(long menuTid);
        /// <summary>
        /// 获取所有父菜单
        /// </summary>
        /// <returns></returns>
        List<SystemMenuSM> GetAllParentMenus();
        /// <summary>
        /// 获取当前用户的所有的菜单
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="menuRights"></param>
        /// <param name="isGod"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetAllRightsMenus(string eid, string menuRights, bool isGod = false);
        /// <summary>
        /// 是否有对当前Url访问的权限
        /// </summary>
        /// <param name="currentUrl"></param>
        /// <param name="menuRights"></param>
        /// <returns></returns>
        long HaveMenuPermission(string currentUrl, string menuRights);
        /// <summary>
        /// 查看是否有操作权限
        /// </summary>
        /// <param name="menuTid"></param>
        /// <param name="roleTid"></param>
        /// <param name="controlname"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        bool HaveActionPermission(long menuTid, long roleTid, string controlname, string actionName);
        /// <summary>
        /// 加载menuTree
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userToken"></param>
        /// <param name="roleSencondeID"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetMenuTree(long roleId, Token userToken, long roleSencondeID = 0);
    }
}