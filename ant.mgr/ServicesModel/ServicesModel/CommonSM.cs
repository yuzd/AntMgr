using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesModel
{
   
    public class UploadImageResult
    {

        public string Md5 { get; set; }
        public string Src { get; set; }
        public string Err { get; set; }
        public string FileName { get; set; }
    }

    public class DbTablesAndColumnsSM
    {
        public DbTablesAndColumnsSM()
        {
            type = "table";
        }
        public string type { get; set; }
        public List<DynamicColumn> columns { get; set; }
        public List<object[]> data { get; set; }
    }

    public class DynamicColumn
    {
        public DynamicColumn(string c)
        {
            this.title = c;
        }
        public string title { get; set; }

    }


    public class CodeGenTable
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string Comment { get; set; }
    }
    public class CodeGenField
    {
        public string Name { get; set; }    
        public string FieldName { get; set; }    
        public string Comment { get; set; }    
    }

   
}
