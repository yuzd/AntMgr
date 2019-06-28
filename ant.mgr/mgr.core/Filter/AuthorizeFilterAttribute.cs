using Autofac.Annotation;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using ServicesModel;
using System;
using System.Linq;
using System.Reflection;
using ant.mgr.core.Controllers;
using DbModel;

namespace ant.mgr.core.Filter
{
    [Bean]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeFilterAttribute : AuthorizeServiceFilterAttribute, IFilterFactory, IFilterMetadata
    {

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            IFilterMetadata requiredService = serviceProvider.GetRequiredService(typeof(AuthorizeFilterAttribute)) as IFilterMetadata;
            if (requiredService != null)
                return requiredService;
            throw new InvalidOperationException();
        }

        public bool IsReusable { get; }
    }
    ///　<summary>
    ///　权限拦截
    ///　</summary>
    public class AuthorizeServiceFilterAttribute : BaseFilterAttribute
    {
        [Autowired]
        public IAccountRespository AccountRespository { get; set; }

        [Autowired]
        public IMenuRespository MenuRespository { get; set; }



        public AuthorizeServiceFilterAttribute()
        {
            AllowAll = false;
        }
        /// <summary>
        /// 是否允许所有人查看
        /// </summary>
        public bool AllowAll { get; set; }

        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (AllowAll) { return; }
            var currentContext = new filterContextInfo(filterContext);
            SystemUsers systemUser;
            //检查是否登录
            //从cookie 拿到token
            var token = CodingUtils.AesDecrypt(WebUtils.GetCookie(GlobalSetting.CurrentLoginUserGuid));
            if (string.IsNullOrEmpty(token))
            {

                Unauthorized(filterContext, currentContext);//跳转登录
                return;
            }

            try
            {

                var tokenObj = new Token(token);
                systemUser = AccountRespository.Entity.FirstOrDefault(r => r.Eid.Equals(tokenObj.Eid));
                ((BaseController)filterContext.Controller).UserToken = tokenObj;
                var smTid = CodingUtils.AesDecrypt(WebUtils.GetCookie(GlobalSetting.CurrentMenu));
                if (!string.IsNullOrEmpty(smTid))
                {
                    ((BaseController)filterContext.Controller).CurrentMenuTid = long.Parse(smTid);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn("OnActionExecuting", ex);
                WebUtils.CookieClear();
                Unauthorized(filterContext, currentContext);//跳转登录
                return;
            }

            //检测用户是否被禁用
            if (systemUser == null || !systemUser.IsActive)
            {
                WebUtils.CookieClear();
                Forbidden(filterContext);//跳转登录
                return;
            }

          

            //检查当前用户是否有访问当前menu的权限
            var currentUrl = currentContext.controllerName + "/" + currentContext.actionName;


            //检查是否是上帝模式//
            if (GlobalSetting.GoldList.Contains(systemUser.Eid))
            {
                return;
            }


            var menuTid = MenuRespository.HaveMenuPermission(currentUrl, systemUser.MenuRights);
            if (menuTid < 0)
            {
                Forbidden(filterContext, string.Concat("[", systemUser.Eid, "-", systemUser.UserName, "]"));
                return;
            }


            if (menuTid == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var refer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(refer))
                    {
                        var s1 = refer.Split('?')[0].Split('/').ToList();
                        s1.Reverse();
                        if (s1.Count < 2) return;
                        var s2 = s1.Take(2).Reverse().ToList();
                        currentUrl = s2[0] + "/" + s2[1];
                        menuTid = MenuRespository.HaveMenuPermission(currentUrl, systemUser.MenuRights);
                    }
                }

            }

            if (menuTid == 0)
            {
                return;
            }

            //走到这里 一定是menu配置过的
            WriteMenuCookie(menuTid);

            if (menuTid == 0) return;

            if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var apiAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttribute<APIAttribute>();
                if (apiAttribute != null)
                {
                    var controlFullName = controllerActionDescriptor.ControllerTypeInfo.Name;
                    var actionFullName = controllerActionDescriptor.MethodInfo.Name;
                    //检查当前Menu 是否配置过了当前的Action
                    var isExist = MenuRespository.HaveActionPermission(menuTid, systemUser.RoleTid, controlFullName, actionFullName);
                    if (!isExist)
                    {
                        Forbidden(filterContext, string.Concat("[", systemUser.Eid, "-", systemUser.UserName, "]"));
                    }
                }
            }


        }

    }


}