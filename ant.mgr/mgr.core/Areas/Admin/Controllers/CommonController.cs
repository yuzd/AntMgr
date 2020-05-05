using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ant.mgr.core.Filter;
using Configuration;
using Infrastructure.Excel;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace ant.mgr.core.Areas.Admin.Controllers
{
    /// <summary>
    /// 公共
    /// </summary>
    [API("公共")]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class CommonController : BaseController
    {
        private readonly ICommonRespository CommonRespository;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_commonRespository"></param>
        public CommonController(ICommonRespository _commonRespository)
        {
            CommonRespository = _commonRespository;
        }

        #region SQL
        [AuthorizeFilter]
        public ActionResult SQL()
        {
            return View();
        }

        /// <summary>
        /// 查询sql 显示Datatable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("执行查询SQL")]
        [HttpPost]
        public JsonResult SQLTable(string db,string sql)
        {
            var result = new ResultJsonInfo<DbTablesAndColumnsSM>();
            result.Data = new DbTablesAndColumnsSM();
            sql = sql.DecodeBase64();
            var table = CommonRespository.SelectSqlExcute(db,sql);
            result.Data.columns = table.Columns
                   .Cast<DataColumn>()
                   .Select(x => new DynamicColumn(x.ColumnName))
                   .ToList();
            result.Data.data = table.Rows.Cast<DataRow>()
                .Select(r => r.ItemArray).ToList();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            return Json(result);
        }


        [API("执行sql导出Excel")]
        [HttpPost, FileDownload]
        [AuthorizeFilter]
        public ActionResult SQLSelect(string db,string sql)
        {
            sql = sql.DecodeBase64();
            var data = CommonRespository.SelectSqlExcute(db,sql);
            var tabelName = $"Report_{DateTime.Now:yyyyMMddHHmmss}";
            data.TableName = tabelName;
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(data.Copy());
            var bytes = dataSet.ToExcel();
            return File(bytes, "application/vnd.ms-excel", tabelName + ".xlsx");
        }


        /// <summary>
        /// 执行Insert update delete 语句
        /// </summary>
        /// <param name="db">指定的db名称</param>
        /// <param name="sql"></param>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("执行Insert,Delete,Update")]
        [HttpPost]
        public JsonResult SQLExcute(string db,string sql)
        {
            var result = new ResultJsonInfo<int>();
            sql = sql.DecodeBase64();
            var respositoryResult = CommonRespository.SQLExcute(db,sql);
            if (string.IsNullOrEmpty(respositoryResult.Item2))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
                result.Data = respositoryResult.Item1;
            }
            else
            {
                result.Status = ResultConfig.Ok;//故意要这样的
                result.Info = respositoryResult.Item2;
                result.Data = respositoryResult.Item1;
            }
            return Json(result);
        }
        #endregion


        #region CodeGen

        /// <summary>
        /// 获取所有的表名称和列
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("获取所有的表名称和列")]
        [HttpPost]
        public JsonResult GetDbTablesAndColumns(string dbName)
        {
            var result = new ResultJsonInfo<string>();
            var respositoryResult = CommonRespository.GetDbTablesAndColumns(dbName);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = "var schema = " + respositoryResult;
            return Json(result);
        }

        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("获取数据库列表")]
        [HttpPost]
        public JsonResult GetDbs()
        {
            var result = new ResultJsonInfo<List<string>>();
            List<string> respositoryResult = CommonRespository.GetDbs();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        /// <summary>
        /// 获取数据库表
        /// </summary>
        /// <returns></returns>

        [AuthorizeFilter]
        [API("获取数据库表")]
        [HttpPost]
        public JsonResult GetDbTables()
        {
            var result = new ResultJsonInfo<List<CodeGenTable>>();
            var respositoryResult = CommonRespository.GetDbTables();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        /// <summary>
        /// 获取数据表下的所有字段
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("获取数据表下的所有字段")]
        [HttpPost]
        public JsonResult GetDbTableColumns(string dbName, string tableName)
        {
            var result = new ResultJsonInfo<List<CodeGenField>>();
            var respositoryResult = CommonRespository.GetDbTablesColumns(dbName, tableName);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, FileDownload]
        [AuthorizeFilter]
        [API("生成代码")]
        public ActionResult CodeGenDown([FromForm] CodeGenVm model)
        {
            var data = CommonRespository.CodeGen(model);
            var arr = model.TableName.Split('→');
            return File(data, System.Net.Mime.MediaTypeNames.Application.Zip, "CodeGen_" + arr[0] + "_" + arr[1] + ".zip");
        }
        #endregion





        #region 代码自动生成

        /// <summary>
        /// 代码生成页面
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("代码生成页面访问")]
        public ActionResult CodeGen()
        {
            return View();
        }

        #endregion




        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("下载文件")]
        public ActionResult Download(string fileName)
        {
            fileName = fileName.Replace("'", "");
            var filePath = WebUtils.GetMapPath(fileName);
            return File(filePath, "application/octet-stream", new FileInfo(filePath).Name);
        }


    }
}