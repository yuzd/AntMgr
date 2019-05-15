using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ant.mgr.core.Controllers;
using DbModel;
using Newtonsoft.Json;

namespace ant.mgr.core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class APIAttribute : Attribute
    {
        public APIAttribute()
        {
            Name = string.Empty;
        }
        public APIAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// API名称
        /// </summary>
        public string Name { get; set; }
    }



    public class APIAttibuteHelper
    {
        //获取当前的程序集里面所有继承了BaseController的类
        //获取当前class上打了API标签的属性 + className
        //在获取当前class里面的所有打了API标签的method + methodName
        public static List<APIDescription> GetAllDescriptions(Assembly current = null,List<SystemPageAction> pageActions =null)
        {
            if (current == null) current = typeof(APIAttibuteHelper).Assembly;
            var result = new List<APIDescription>();
            var types = current.GetExportedTypes();
            var maps = (from t in types
                        where t.IsClass && t.BaseType == typeof(BaseController) &&
                              !t.IsAbstract && !t.IsInterface
                        select new
                        {
                            ClassName = t.Name,
                            Type = t,
                            Attribute = t.GetCustomAttribute<APIAttribute>()
                        }).ToArray();

            var div = new Dictionary<string,List<string>>();
            if (pageActions != null)
            {
                div = pageActions.GroupBy(r => r.ControlName)
                    .ToDictionary(r => r.Key, y => y.Select(r => r.ActionName).ToList());
            }
            foreach (var item in maps)
            {
                APIDescription classDescription = new APIDescription
                {
                    APIName = item.Attribute != null && !string.IsNullOrEmpty(item.Attribute.Name) ? item.Attribute.Name : string.Empty,
                    Name = item.ClassName,
                    MethodList = new List<APIDescription>()
                };
                var methodInfos = item.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(r => new
                {
                    MethodName = r.Name,
                    Attribute = r.GetCustomAttribute<APIAttribute>()
                }).Where(r => r.Attribute != null).ToList();

                var divAction = new List<string>();
                div.TryGetValue(item.ClassName, out divAction);
                foreach (var method in methodInfos)
                {
                    classDescription.MethodList.Add(new APIDescription
                    {
                        APIName = !string.IsNullOrEmpty(method.Attribute.Name) ? method.Attribute.Name : string.Empty,
                        Name = method.MethodName,
                        ParentName = item.ClassName,
                        Check = divAction!=null&&divAction.Contains(method.MethodName)
                    });
                }

                if (classDescription.MethodList.Count > 0)
                {
                    classDescription.MethodList = classDescription.MethodList.OrderBy(r => r.Name).ToList();
                    result.Add(classDescription);
                }
            }

            result = result.OrderBy(r => r.Name).ToList();
           
            return result;
        }



    }

    public class APIDescription
    {
      
        public string Name { get; set; }

        [JsonProperty("name")]
        public string DisplayName => this.APIName + "[" + this.Name + "]";

        public int id { get; set; }
        public int pid { get; set; }
        public string APIName { get; set; }
        public string Action { get; set; }
        public string ParentName { get; set; }

        [JsonProperty("checked")]
        public bool Check { get; set; }

        [JsonProperty("children")]
        public List<APIDescription> MethodList { get; set; }
    }
}