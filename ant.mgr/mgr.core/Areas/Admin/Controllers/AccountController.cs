using System.Collections.Generic;
using System.Threading.Tasks;
using ant.mgr.core.Filter;
using Configuration;
using DbModel;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace ant.mgr.core.Areas.Admin.Controllers
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [API("用户")]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly IAccountRespository AccountRespository;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_accountRespository"></param>
        public AccountController(IAccountRespository _accountRespository)
        {
            AccountRespository = _accountRespository;
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 个人信息页面
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("用户信息页面访问")]
        public async Task<ActionResult> UserDetail()
        {
            var currentUser = await AccountRespository.GetUserInfo(UserToken);
            ViewBag.UserName = currentUser?.UserName;
            ViewBag.Phone = currentUser?.Phone;
            return View();
        }

        /// <summary>
        /// 更新个人信息
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [API("更新个人信息")]
        [AuthorizeFilter]
        public async Task<ActionResult> UpdateUserInfo([FromForm] SystemUsers user)
        {
            var result = new ResultJsonNoDataInfo();
            user.Eid = UserToken.Eid;
            var respositoryResult = await AccountRespository.UpdateUserInfo(user);
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
        /// 更新登录密码
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [API("更新登录密码")]
        [AuthorizeFilter]
        public async Task<ActionResult> UpdatePwd([FromForm] UpdatePwdVm user)
        {
            var result = new ResultJsonNoDataInfo();
            user.Eid = UserToken.Eid;
            var respositoryResult = await AccountRespository.UpdatePwd(user);
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
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            SessionClear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LogOn([FromForm] LogOnVM info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.LogOn(info);
            if (respositoryResult.Item1)
            {

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult.Item2 ?? ResultConfig.FailMessage;
            }
            return Json(result);
        }

        /// <summary>
        /// 用户列表页面
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("用户列表页面访问")]
        public ActionResult UserList()
        {
            return View();
        }

        /// <summary>
        /// 获取所有的用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeFilter]
        [HttpPost]
        [API("获取所有的用户")]
        public async Task<JsonResult> GetUserList([FromForm] AccountVm model)
        {
            var result = new SearchResult<List<UserSM>>();
            var respositoryResult = await AccountRespository.GetUserList(model, UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Rows = respositoryResult.Item2;
            result.Total = respositoryResult.Item1;
            return Json(result);
        }

        /// <summary>
        /// 给用户赋予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("给用户赋予角色")]
        public async Task<JsonResult> UserAddRole([FromForm] UserAddRoleVm info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserAddRole(info);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 手动添加用户
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [API("手动添加用户")]
        public async Task<JsonResult> UserAdd([FromForm] SystemUsers info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserAdd(info,UserToken);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="UserTid"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("禁用用户")]
        public async Task<JsonResult> UserDelete(long UserTid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserDelete(UserTid);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 改变某个属性值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("改变某个属性值")]
        public async Task<JsonResult> ChangeField([FromForm] ChangeFieldVm info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.ChangeField(info);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = string.Empty;
            }
            else
            {
                result.Status = ResultConfig.Ok;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        #region Private
        private void SessionClear()
        {
            WebUtils.CookieClear();
        }

        #endregion

    }

}