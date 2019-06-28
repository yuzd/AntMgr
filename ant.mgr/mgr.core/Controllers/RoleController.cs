using System.Collections.Generic;
using System.Threading.Tasks;
using ant.mgr.core.Filter;
using Configuration;
using DbModel;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace ant.mgr.core.Controllers
{
    [AuthorizeFilter]
    [API("角色")]
    public class RoleController : BaseController
    {
        private readonly IRoleRespository RoleRespository;
        public RoleController(IRoleRespository _roleRespository)
        {
            RoleRespository = _roleRespository;
        }

        public ViewResult RoleList()
        {
            return View();
        }

        public ViewResult RoleAction()
        {
            return View();
        }

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        [API("分页获取所有的角色")]
        public async Task<JsonResult> GetRoleList(RoleVm model)
        {
            var result = new SearchResult<List<SystemRole>>();
            var respositoryResult = await RoleRespository.GetList(model, UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Rows = respositoryResult.Item2;
            result.Total = respositoryResult.Item1;
            return Json(result);
        }

        /// <summary>
        /// 没有分页
        /// </summary>
        /// <returns></returns>
        [API("获取所有的角色")]
        public async Task<JsonResult> GetAllRoleList()
        {
            var result = new ResultJsonInfo<List<SystemRole>>();
            var respositoryResult = await RoleRespository.GetAllRoleList(UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [API("新增角色")]
        public async Task<JsonResult> AddRole([ModelBinder(typeof(JsonNetBinder)),FromForm]AddRoleVm role)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await RoleRespository.AddRole(role, UserToken);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult) ? ResultConfig.FailMessage : respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 获取当前所有的action
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSystemUserActions()
        {
            var result = new ResultJsonInfo<SystemAction>();
            var respositoryResult = await RoleRespository.GetSystemUserActions(UserToken.Eid, UserToken.RoleTid, base.CurrentMenuTid);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        [HttpPost]
        [API("页面与接口关联")]
        public async Task<JsonResult> AddRoleActions([ModelBinder(typeof(JsonNetBinder)),FromForm]RoleAction model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await RoleRespository.AddRoleActions(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult) ? ResultConfig.FailMessage : respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [API("删除角色")]
        public async Task<JsonResult> DeleteRole([FromForm] long tid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await RoleRespository.DeleteRole(tid, UserToken);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult) ? ResultConfig.FailMessage : respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 权限与接口关联配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetRoleActions(RoleAction model)
        {
            var result = new ResultJsonInfo<List<APIDescription>>();
            var respositoryResult = await RoleRespository.GetRoleActions(model);
            var allDescriptions = APIAttibuteHelper.GetAllDescriptions(null, respositoryResult);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = allDescriptions;
            return Json(result);
        }


    }
}