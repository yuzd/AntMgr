using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configuration;
using DbModel;
using ant.mgr.core.Filter;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using Infrastructure.Excel;
using ViewModels.Result;
using ViewModels.Reuqest;
using ant.mgr.core.Areas.Admin.Controllers;

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
        [API("页面访问")]
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
                result.Info = respositoryResult;
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

        /// <summary>
        /// 下载导入{{ModelName}}的Excel模板
        /// </summary>
        [API("下载导入Excel模板")]
        public ActionResult ExcelTemplete()
        {
            var bytes = {{ModelClassName}}Respository.ExcelTemplete();
            if (!string.IsNullOrEmpty(bytes.Item1))
            {
                var result = new ResultJsonNoDataInfo {Status = ResultConfig.Fail, Info = bytes.Item1 };
                return Json(result);
            }
            return File(bytes.Item2, "application/vnd.ms-excel", "{{ModelClassName}}Templete.xlsx");
        }

        /// <summary>
        /// 根据{{ModelName}}的Excel模板导入
        /// </summary>
        [API("导入Excel")]
        public JsonResult Upload()
        {
            using var inputFileStream = Request.Form.Files[0].OpenReadStream();
            var result = new ResultJsonInfo<string>();
            var respositoryResult = {{ModelClassName}}Respository.UseTransactionUpload(inputFileStream, UserToken.Code);
            result.Status = !respositoryResult.Item1?ResultConfig.Fail: ResultConfig.Ok;
            result.Info = respositoryResult.Item2;
            return Json(result);
        }

        /// <summary>
        /// 导出{{ModelName}}的Excel
        /// </summary>
        [API("导出Excel")]
        [HttpPost, FileDownload]
        public async Task<ActionResult> Export(SchoolVm model)
        {
            var data = await {{ModelClassName}}Respository.Export(model);
            var tabelName = $"{{ModelClassName}}_{DateTime.Now:yyyyMMddHHmmss}";
            var bytes = ExcelHelper.ExportExcel(data);
            return File(bytes, "application/vnd.ms-excel", tabelName+".xlsx");
        }
    }

}