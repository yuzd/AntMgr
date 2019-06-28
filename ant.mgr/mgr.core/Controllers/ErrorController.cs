using Configuration;
using Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Result;

namespace ant.mgr.core.Controllers
{
    /// <summary>
    /// 错误
    /// </summary>
    public class ErrorController : BaseController
    {

        public IHttpContextAccessor HttpContextAccessor { get; }

        public ErrorController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        [Route("error/404")]
        public ActionResult Http404()
        {
            if (HttpContextAccessor.HttpContext.Request.IsAjaxRequest())
            {
                return Http405();
            }
            return View();
        }

        [Route("Http403")]
        public ActionResult Http403(string userInfo)
        {
            ViewBag.userInfo = userInfo;
            return View();
        }

        [Route("NoPower")]
        public ActionResult NoPower(string acionInfo)
        {
            ViewBag.ActionInfo = acionInfo;
            return View();
        }

        /// <summary>
        /// 未登录
        /// </summary>
        [Route("NoLogin")]
        public ActionResult NoLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// 没有权限
        /// </summary>
        /// <returns>ActionResult.</returns>
        [Route("error/401")]
        public JsonResult Http401()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForNoPower;
            return Json(result);
        }

        /// <summary>
        /// 系统错误
        /// </summary>
        /// <returns></returns>
        [Route("error/500")]
        public JsonResult Http500()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForSystem;
            return Json(result);
        }
        [Route("Http405")]
        public ActionResult Http405()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForNotFound;
            return Json(result);
        }
    }
}