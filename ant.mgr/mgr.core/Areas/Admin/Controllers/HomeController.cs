using System.Collections.Generic;
using ant.mgr.core.Filter;
using Configuration;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace ant.mgr.core.Areas.Admin.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilterAttribute))]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IMenuRespository MenuRespository;

        public HomeController(IMenuRespository _menuRespository)
        {
            MenuRespository = _menuRespository;
        }
        public ActionResult Index()
        {
            var roleName = UserToken.RoleName;
            if (string.IsNullOrEmpty(roleName))
            {
                roleName = "无权限";
            }
            if (GlobalSetting.GoldList.Contains(UserToken.Eid.ToLower()))
            {
                roleName = "上帝模式";
            }

            ViewBag.Eid = UserToken.Eid;
            ViewBag.RoleName = roleName;
            ViewBag.UserName = UserToken.Code;
            return View();
        }



        #region 菜单


        /// <summary>
        /// 获取所有的Menu
        /// </summary>
        /// <returns></returns>
        [API("菜单页面访问")]
        public ActionResult MenuList()
        {
            return View();
        }


        /// <summary>
        /// 加载Menu Tree
        /// </summary>
        /// <returns></returns>
        [API("加载Menu Tree")]
        public JsonResult GetMenuTree([FromForm]string roleId = null)
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var respositoryResult = MenuRespository.GetMenuTree(string.IsNullOrEmpty(roleId) ? 0 : long.Parse(roleId), UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }
        [API("获取API集合")]
        public JsonResult GetControllerActions([FromForm]string roleId = null)
        {
            var result = new ResultJsonInfo<List<APIDescription>>();
            var respositoryResult = APIAttibuteHelper.GetAllDescriptions();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        [API("加载所有的菜单和按钮")]
        public JsonResult GetMenuActionTree()
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            if (!GlobalSetting.GoldList.Contains(UserToken.Eid))
            {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessageForNoPower;
                result.Data = new List<SystemMenuSM>();
                return Json(result);
            }
            var respositoryResult = MenuRespository.GetMenuTree(0, UserToken);
            if (respositoryResult == null)
            {
                result.Status = ResultConfig.Fail;
                result.Info = "没有配置任何菜单";
                result.Data = new List<SystemMenuSM>();
                return Json(result);
            }
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }
        /// <summary>
        /// 获取子菜单
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        [API("获取子菜单")]
        public JsonResult GetSubMenu([FromForm]long menuTid)
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var respositoryResult = MenuRespository.GetSubMenus(menuTid);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        [API("获取全部一级菜单")]
        public JsonResult GetAllParentMenus()
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var allParentMenuList = MenuRespository.GetAllParentMenus();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = allParentMenuList;
            return Json(result);
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <returns></returns>
        [API("新增菜单")]
        public JsonResult AddMenu([FromForm] AddMenuVm model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.AddMenu(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <returns></returns>
        [API("获取修改的菜单详情")]
        public JsonResult GetEditMenu([FromForm] AddMenuVm model)
        {
            var result = new ResultJsonInfo<SystemMenuSM>();
            var CurrentMenu = MenuRespository.GetCurrentMenu(model.Tid);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = CurrentMenu;
            return Json(result);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [API("修改菜单")]
        public JsonResult UpdateMenu([FromForm]AddMenuVm model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.UpdateMenu(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }
        /// <summary>
        /// 禁用菜单
        /// </summary>
        /// <returns></returns>
        [API("禁用菜单")]
        public JsonResult DisableMenu([FromForm] long menuTid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.DisableMenu(menuTid);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        #endregion




    }
}