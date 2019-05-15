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
    /// 
    /// </summary>
    public interface IMenuRespository : IRepository<SystemMenu>
    {
        string DisableMenu(long menuTid);
        SystemMenuSM GetCurrentMenu(long menuTid);
        string UpdateMenu(AddMenuVm model);
        string AddMenu(AddMenuVm model);
        List<SystemMenuSM> GetSubMenus(long menuTid);
        List<SystemMenuSM> GetAllParentMenus();
        List<SystemMenuSM> GetAllRightsMenus(string eid, string menuRights, bool isGod = false);

        long HaveMenuPermission(string currentUrl, string menuRights);
        bool HaveActionPermission(long menuTid, long roleTid, string controlname, string actionName);
        List<SystemMenuSM> GetMenuTree(long roleId, Token userToken, long roleSencondeID = 0);
    }
}