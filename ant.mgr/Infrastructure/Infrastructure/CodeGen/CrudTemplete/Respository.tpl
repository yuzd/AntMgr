using AntData.ORM;
using AntData.ORM.Linq;
using Autofac.Annotation;
using Castle.DynamicProxy;
using Configuration;
using DbModel.Mysql;
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


namespace Repository
{
    [Bean(typeof(I{{ModelClassName}}Respository), Interceptor = typeof(AsyncInterceptor))]
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



    }
}
