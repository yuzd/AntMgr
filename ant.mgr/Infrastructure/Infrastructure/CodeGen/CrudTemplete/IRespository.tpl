using DbModel.Mysql;
using ViewModels.Reuqest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    
    /// <summary>
    /// {{ModelName}}接口
    /// </summary>
    public interface I{{ModelClassName}}Respository : IRepository<{{ModelClassName}}>
    {
        Task<Tuple<long, List<{{ModelClassName}}>>> Get{{ModelClassName}}List({{ModelClassName}}Vm model);
        string Add{{ModelClassName}}({{ModelClassName}} model);
        Task<string> Del{{ModelClassName}}(long tid);
    }
}