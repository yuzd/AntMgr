using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModel;
using Infrastructure.Excel;

namespace ServicesModel
{

    //如果要导出excel的第一列：标题是对应的中文 可以增加 ExcelClass 标签 (中文名称,对应的dbmodel的属性名称，排序值[越小越在前面])
    {% for field in ModelFields %}
    [ExcelClass("{{field.Comment}}", Column = "{{field.Name}}", OrderRule = {{forloop.index}})]
    {% endfor %}
    public class {{ModelClassName}}SM : {{ModelClassName}}
    {
        
        //也可以增加自定义列 并打上 ExcelField 标签 (中文名称,排序值)
        //[ExcelField("中文名称", OrderRule = 10)]
        //public string Other { get; set; }

    }

   
}
