using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServicesModel;

namespace ant.mgr.core.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [EnableCors("Any")]
    public class BaseController : Controller
    {
        /// <summary>
        /// 用户登录态
        /// </summary>
        public Token UserToken { get; set; }

        /// <summary>
        /// 当前菜单Tid
        /// </summary>
        public long CurrentMenuTid { get; set; }

    }

}