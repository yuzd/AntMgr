using Configuration;
using Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Result;

namespace ant.mgr.core.Areas.Admin.Controllers
{
    /// <summary>
    /// 错误
    /// </summary>
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class ErrorController : BaseController
    {

        public IHttpContextAccessor HttpContextAccessor { get; }

        public ErrorController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        [Route("admin/error/404")]
        public ActionResult Http404()
        {
            if (HttpContextAccessor.HttpContext.Request.IsAjaxRequest())
            {
                return Http405();
            }
            return View();
        }

        public ActionResult Http403(string userInfo)
        {
            ViewBag.userInfo = userInfo;
            return View();
        }


        /// <summary>
        /// 未登录
        /// </summary>
        public ActionResult NoLogin()
        {
            return RedirectToAction("Login", "Account",new {area="Admin"});
        }

        /// <summary>
        /// 没有权限
        /// </summary>
        /// <returns>ActionResult.</returns>
        [Route("admin/error/401")]
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
        [Route("admin/error/500")]
        public JsonResult Http500(string exception =null)
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForSystem + (exception!=null?$"【Exception:{exception}】":"");
            return Json(result);
        }

        public ActionResult Http405()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForNotFound;
            return Json(result);
        }
    }
}