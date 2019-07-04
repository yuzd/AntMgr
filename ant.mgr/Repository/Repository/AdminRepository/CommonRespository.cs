using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Mapping;
using Autofac.Annotation;
using Castle.DynamicProxy;
using DbModel;
using Infrastructure.CodeGen;
using Infrastructure.Logging;
using Newtonsoft.Json;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ViewModels.Reuqest;
using Infrastructure;
using System.Text;

namespace Repository
{
    /// <summary>
    /// 公共处理
    /// </summary>
    [Bean(typeof(ICommonRespository), Interceptor = typeof(AsyncInterceptor))]
    public class CommonRespository : BaseRepository, ICommonRespository
    {


        /// <summary>
        /// 获取所有的Table和Columns
        /// </summary>
        /// <returns></returns>
        public string GetDbTablesAndColumns()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            List<string> tables = this.DB.Query<string>("show tables").ToList();
            foreach (var table in tables)
            {
                var columns = getAllFields(table);
                result.Add(table, columns);
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <returns></returns>
        public List<CodeGenTable> GetDbTables()
        {

            return this.GetDbTabless();
        }

        /// <summary>
        /// 获取表下面所有的字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<CodeGenField> GetDbTablesColumns(string tableName)
        {
            return this.GetDbModels(tableName);
        }







        /// <summary>
        /// 自动生成代码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public byte[] CodeGen(CodeGenVm model)
        {
            return GeneratorCodeHelper.CodeGenerator(model.TableName, model.Columns);
        }


        /// <summary>
        /// 获取表里面所有的字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<string> getAllFields(string tableName)
        {
            var columns = this.DB.Query<string>(" SHOW COLUMNS FROM " + tableName).ToList();
            return columns;
        }


        /// <summary>
        /// 获取所有的DBTable
        /// </summary>
        private List<CodeGenTable> GetDbTabless()
        {
            var result = new List<CodeGenTable>();
            try
            {

                var modelAss = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => (assembly.GetName().Name.Equals("DbModel")));
                if (modelAss == null)
                {
                    throw new ArgumentException("assemblys");
                }
                var types = modelAss.GetExportedTypes();
                var targetClass = (from t in types
                                   where t.BaseType == typeof(LinqToDBEntity) &&
                                       !t.IsAbstract &&
                                       !t.IsInterface
                                   select t).ToArray();


                foreach (var tt in targetClass)
                {
                    var tart = tt.GetCustomAttribute<TableAttribute>();
                    if (tart == null)
                    {
                        continue;
                    }

                    var comment = tart.Comment;
                    if (string.IsNullOrEmpty(comment))
                    {
                        comment = string.Empty;
                        LogHelper.Debug("GetDbTabless", tart.Name + "表的Comment为空!!");
                    }
                    result.Add(new CodeGenTable
                    {
                        Name = tt.Name,
                        TableName = tart.Name,
                        Comment = comment.Replace(",", "").Replace("→", "")
                    });
                }
                return result.OrderBy(r => r.Name).ToList();
            }
            catch (Exception ex)
            {

                LogHelper.Warn("GetDbTabless", "可能有表的Comment为空导致", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有的DBClass
        /// </summary>
        private List<CodeGenField> GetDbModels(string tableName)
        {
            var modelAss = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => (assembly.GetName().Name.Equals("DbModel")));
            if (modelAss == null)
            {
                throw new ArgumentException("assemblys");
            }
            var types = modelAss.GetExportedTypes();
            var targetClass = (from t in types
                               where t.BaseType == typeof(LinqToDBEntity) &&
                                          !t.IsAbstract &&
                                          !t.IsInterface && t.Name.Equals(tableName)
                               select t).FirstOrDefault();

            if (targetClass == null)
            {
                throw new ArgumentException("targetClass");
            }
            
            var properties = targetClass.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(e => e.CanWrite).ToArray();

            var result = (from item in properties
                          let r = item.GetCustomAttribute<ColumnAttribute>()
                          select new CodeGenField
                          {
                              Name = item.Name,
                              FieldName = r.Name,
                              Comment = string.IsNullOrEmpty(r.Comment) ? "" : r.Comment.Replace(",", "").Replace("→", "")
                          }).ToList();
            return result.OrderBy(r => r.Name).ToList();
        }


    }
}
