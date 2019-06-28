using Configuration;
using Infrastructure.Logging;
using Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using ViewModels.Result;

namespace ant.mgr.core.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HandleAjaxRequestException(filterContext);
            }
        }

        private void HandleAjaxRequestException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            var errorMsg = filterContext.Exception.InnerException != null
                ? filterContext.Exception.InnerException.Message
                : filterContext.Exception.Message;

            filterContext.Result = new JsonResult(new ResultJsonNoDataInfo()
            {
                Info = errorMsg,
                Status = ResultConfig.Fail
            });
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            LogHelper.Warn(nameof(GlobalExceptionFilter.HandleAjaxRequestException), filterContext.Exception);
        }
    }
}
