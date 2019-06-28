using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configuration;
using DbModel.Mysql;
using Infrastructure.Web;
using ant.mgr.core.Filter;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace ant.mgr.core.Controllers
{
    [AuthorizeFilter]
    [API("{{ModelName}}")]
    public class {{ModelClassName}}Controller : BaseController
    {
        private readonly I{{ModelClassName}}Respository {{ModelClassName}}Respository;
        public {{ModelClassName}}Controller(I{{ModelClassName}}Respository _{{ModelClassName}}Respository)
        {
            {{ModelClassName}}Respository = _{{ModelClassName}}Respository;
        }

        /// <summary>
        /// 进入{{ModelName}}页面
        /// </summary>
        /// <returns></returns>
        public ActionResult {{ModelClassName}}()
        {
            return View();
        }

        /// <summary>
        /// 获取{{ModelName}}列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [API("获取{{ModelName}}列表")]
        public async Task<JsonResult> Get{{ModelClassName}}List({{ModelClassName}}Vm model)
        {
            var result = new SearchResult<List<{{ModelClassName}}>>();
            var respositoryResult = await {{ModelClassName}}Respository.Get{{ModelClassName}}List(model);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Rows = respositoryResult.Item2;
            result.Total = respositoryResult.Item1;
            return Json(result);
        }

        /// <summary>
        /// 添加或修改{{ModelName}}
        /// </summary>
        /// <returns></returns>
        [API("添加或修改{{ModelName}}")]
        public JsonResult Add{{ModelClassName}}([FromForm] {{ModelClassName}} model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = {{ModelClassName}}Respository.Add{{ModelClassName}}(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult) ? ResultConfig.FailMessage : respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 删除{{ModelName}}
        /// </summary>
        /// <returns></returns>
        [API("删除{{ModelName}}")]
        public async Task<JsonResult> Del{{ModelClassName}}([FromForm] long tid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await {{ModelClassName}}Respository.Del{{ModelClassName}}(tid);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult) ? ResultConfig.FailMessage : respositoryResult;
            }
            return Json(result);
        }


    }

}