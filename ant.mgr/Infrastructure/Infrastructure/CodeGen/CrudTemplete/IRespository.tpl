using ViewModels.Reuqest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DbModel;
using ServicesModel;

namespace Repository.Interface
{
    
    /// <summary>
    /// {{ModelName}}接口
    /// </summary>
    public interface I{{ModelClassName}}Respository : IRepository<{{ModelClassName}}>
    {
        /// <summary>
        /// 获取{{ModelName}}列表
        /// </summary>
        Task<Tuple<long, List<{{ModelClassName}}>>> Get{{ModelClassName}}List({{ModelClassName}}Vm model);

        /// <summary>
        /// 添加或修改{{ModelName}}
        /// </summary>
        string Add{{ModelClassName}}({{ModelClassName}} model);

        /// <summary>
        /// 删除{{ModelName}}
        /// </summary>
        Task<string> Del{{ModelClassName}}(long tid);

        /// <summary>
        /// 下载导入{{ModelName}}的Excel模板
        /// </summary>
        Tuple<string, byte[]> ExcelTemplete();

        /// <summary>
        /// 根据{{ModelName}}的Excel模板导入
        /// </summary>
        Tuple<bool, string> UseTransactionUpload(Stream inputFileStream, string userName);

        /// <summary>
        /// 导出{{ModelName}}的Excel
        /// </summary>
        Task<List<{{ModelClassName}}SM>> Export({{ModelClassName}}Vm model);
    }
}