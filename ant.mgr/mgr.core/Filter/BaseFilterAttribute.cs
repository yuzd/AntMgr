using System;
using Configuration;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ViewModels.Result;

namespace ant.mgr.core.Filter
{
    ///　<summary>
    ///　权限拦截
    ///　</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class BaseFilterAttribute : ActionFilterAttribute
    {


        #region 未授权和登录处理

        protected void Forbidden(ActionExecutingContext filterContext, string userInfo = null)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult(new ResultJsonNoDataInfo()
                {
                    Info = ResultConfig.FailMessageForNoPower,
                    Status = ResultConfig.Fail
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(userInfo))
                {
                    var http403 = new RouteValueDictionary(new
                    {
                        action = "http403",
                        controller = "error",
                        userInfo = userInfo
                    });
                    filterContext.Result = new RedirectToRouteResult(http403);
                    return;
                }
                filterContext.Result = new RedirectResult("~/Error/http403");
            }
        }

        /// <summary>
        /// 未授权处理
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="currentContext"></param>
        protected void Unauthorized(ActionExecutingContext filterContext, filterContextInfo currentContext)
        {
            //session失效
            if (filterContext.HttpContext.Request.IsAjaxRequest()) //if Ajax request
            {
                filterContext.Result = new JsonResult(new ResultJsonNoDataInfo()
                {
                    Info = ResultConfig.FailMessageForNoPower,
                    Status = ResultConfig.NoPower
                });
            }
            else
            {
                var requestUrl = filterContext.HttpContext.Request.SafeGetDisplayUrl();
                if ((currentContext.controllerName.ToLower().Equals("home") &&
                     currentContext.actionName.ToLower().Equals("index")))
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                    return;
                }
                else if (!string.IsNullOrEmpty(requestUrl) && !requestUrl.ToLower().Contains("home/index"))
                {
                    filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + WebUtils.UrlEncode(requestUrl));
                    return;
                }
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
        #endregion


        #region WriteMenuCookie

        protected void WriteMenuCookie(long menuTid)
        {
            string strName = GlobalSetting.CurrentMenu;
            string strValue = CodingUtils.AesEncrypt(menuTid.ToString());
            WebUtils.WriteCookie(strName, strValue);
        }



        #endregion
    }

    public class filterContextInfo
    {
        public filterContextInfo(ActionExecutingContext filterContext)
        {
            #region 获取链接中的字符


            //获取模块名称
            //  module = filterContext.HttpContext.Request.Url.Segments[1].Replace('/', ' ').Trim();

            //获取 controllerName 名称
            controllerName = filterContext.RouteData.Values["controller"].ToString();

            //获取ACTION 名称
            actionName = filterContext.RouteData.Values["action"].ToString();

            #endregion 获取链接中的字符
        }

   

        /// <summary>
        /// 获取模块名称
        /// </summary>
        public string module { get; set; }

        /// <summary>
        /// 获取 controllerName 名称
        /// </summary>
        public string controllerName { get; set; }

        /// <summary>
        /// 获取ACTION 名称
        /// </summary>
        public string actionName { get; set; }
    }
}