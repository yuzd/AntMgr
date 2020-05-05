using AntData.ORM;
using AntData.ORM.Linq;
using Autofac.Annotation;
using Repository.Interceptors;
using Configuration;
using DbModel;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels.Reuqest;
using Infrastructure.Excel;

namespace Repository
{
    [Component]
    public class {{ModelClassName}}Respository : BaseRepository<{{ModelClassName}}>, I{{ModelClassName}}Respository
    {

        /// <summary>
        /// 获取{{ModelName}}列表
        /// </summary>
        /// <param name="model">{{ModelName}}-VM对象</param>
        /// <returns></returns>
        public async Task<Tuple<long, List<{{ModelClassName}}>>> Get{{ModelClassName}}List({{ModelClassName}}Vm model)
        {
            if (model == null) { return new Tuple<long, List<{{ModelClassName}}>>(0, new List<{{ModelClassName}}>()); }

            var totalQuery = this.Entity;
            var listQuery = this.Entity;

            //这里开始写条件
            
            var total = totalQuery.CountAsync();
            var list = await listQuery.DynamicOrderBy(string.IsNullOrEmpty(model.OrderBy) ? "DataChangeLastTime" : model.OrderBy,
                           model.OrderSequence)
                           .Skip((model.PageIndex - 1) * model.PageSize)
                           .Take(model.PageSize)
                           .ToListAsync();

            return new Tuple<long, List<{{ModelClassName}}>>(await total, list);
        }

        /// <summary>
        /// 导出{{ModelName}}
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<{{ModelClassName}}SM>> Export(SchoolVm model)
        {
            if (model == null) return new List<{{ModelClassName}}SM>();

            var listQuery = this.Entity;

            //这里开始写条件


            var list = await listQuery.DynamicOrderBy(string.IsNullOrEmpty(model.OrderBy) ? "DataChangeLastTime" : model.OrderBy,
                    model.OrderSequence)
                .Select(r=>new {{ModelClassName}}SM
                {
                 {% for field in ModelFields %}
                    {{field.Name}} = r.{{field.Name}},
                 {% endfor %}
                })
                .ToListAsync();

            return list;
        }

        /// <summary>
        /// 新增或修改{{ModelName}}
        /// </summary>
        /// <param name="model">{{ModelName}}-对象</param>
        /// <returns></returns>
        public string Add{{ModelClassName}}({{ModelClassName}} model)
        {
            if (model == null)
            {
                return Tip.BadRequest;
            }

             //写入其他的校验规则 或者其他逻辑 比如 初始值设置

            if (model.Tid > 0)
            {
                
                model.DataChangeLastTime = DateTime.Now;
                //修改
                var update = this.DB.Update(model) > 0;
                if (!update)
                {
                    return Tip.UpdateError;
                }
            }
            else
            {
                var result = this.DB.Insert(model) > 0;
                if (!result)
                {
                    return Tip.InserError;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 删除{{ModelName}}
        /// </summary>
        /// <param name="tid">{{ModelName}}的主键</param>
        /// <returns></returns>
        public async Task<string> Del{{ModelClassName}}(long tid)
        {
            var result = await this.Entitys.Get<{{ModelClassName}}>().Where(r => r.Tid.Equals(tid)).DeleteAsync() > 0;
            return !result ? Tip.DeleteError : string.Empty;
        }

        /// <summary>
        /// 下载导入Excel模板
        /// </summary>
        /// <returns></returns>
        public Tuple<string, byte[]> ExcelTemplete()
        {
            var ignoreColumns = new string[] { "DataChangeLastTime" };
            var fields = this.GetTableFileds<{{ModelClassName}}>(ignoreColumns);
            if (!string.IsNullOrEmpty(fields.Item1))
            {
                return Tuple.Create<string, byte[]>(fields.Item1, null);
            }
            var bytes = ExcelHelper.ToExcel(fields.Item2);
            return Tuple.Create<string, byte[]>(null, bytes);
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="inputFileStream"></param>
        /// <param name="userName">当前操作人姓名</param>
        /// <returns></returns>
        public Tuple<bool, string> UseTransactionUpload(Stream inputFileStream, string userName)
        {
            return CommonUpload(inputFileStream, (model) =>
            {
                //这里你可以对数据进行加工或者字段的验证
                //if (string.IsNullOrEmpty(model.Name))
                //{
                    //return "名称不能为空";
                //}

                //或者数据加工
                //model.UpdateUser = userName;
                
                //校验没错
                return string.Empty;
            });
        }
    }
}
