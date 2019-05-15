using Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Threading.Tasks;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace ant.mgr.core.Middleware
{

    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            RequestContext.Instance.StartRequestContext();
            context.Request.EnableRewind();
            await _next(context);
        }
    }
}
