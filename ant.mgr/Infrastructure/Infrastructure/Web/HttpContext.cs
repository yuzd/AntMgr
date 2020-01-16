using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web
{
    public static class HttpContext
    {
        public static ILifetimeScope ServiceProvider;
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                IHttpContextAccessor factory = ServiceProvider.Resolve(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                Microsoft.AspNetCore.Http.HttpContext context = factory?.HttpContext;
                return context;
            }
        }
    }
}
