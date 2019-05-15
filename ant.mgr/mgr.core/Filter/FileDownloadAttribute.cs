using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ant.mgr.core.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FileDownloadAttribute : BaseFilterAttribute
    {
        public FileDownloadAttribute(string cookieName = "fileDownload", string cookiePath = "/")
        {
            CookieName = cookieName;
            CookiePath = cookiePath;
        }

        public string CookieName { get; set; }

        public string CookiePath { get; set; }

        /// <summary>
        /// If the current response is a FileResult (an MVC base class for files) then write a
        /// cookie to inform jquery.fileDownload that a successful file download has occured
        /// </summary>
        /// <param name="filterContext"></param>
        private void CheckAndHandleFileResult(ActionExecutedContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var response = httpContext.Response;

            if (filterContext.Result is FileResult)
            {
                var CookieOps = new CookieOptions
                {
                    Path = CookiePath
                };
                response.Cookies.Append(CookieName, "true", CookieOps);
            }
            else
            {
                if (httpContext.Request.Cookies[CookieName] != null)
                {
                    var CookieOps = new CookieOptions
                    {
                        Path = CookiePath,
                        Expires = DateTime.Now.AddYears(-1),
                    };
                    response.Cookies.Append(CookieName, "true", CookieOps);
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            CheckAndHandleFileResult(filterContext);

            base.OnActionExecuted(filterContext);
        }
    }
}