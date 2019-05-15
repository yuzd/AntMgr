using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web
{
    public static class HttpContext
    {
        public static IServiceProvider ServiceProvider;
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                IHttpContextAccessor factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor)) as IHttpContextAccessor;
                Microsoft.AspNetCore.Http.HttpContext context = factory?.HttpContext;
                return context;
            }
        }
    }
}
