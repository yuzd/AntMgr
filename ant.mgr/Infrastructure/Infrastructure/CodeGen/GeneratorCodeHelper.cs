using DotLiquid;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure.CodeGen
{
    /// <summary>
    /// 自动生成代码
    /// </summary>
    public static class GeneratorCodeHelper
    {
        /// <summary>
        /// code
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columns">表字段</param>
        public static byte[] CodeGenerator(string tableName, List<string> columns)
        {
            //ModelClassName
            //ModelName
            //ModelFields  Name Comment
            byte[] data;
            var _modelName = tableName.Split('→')[1];
            var _modelClassName = tableName.Split('→')[0];
            var obj = new
            {
                ModelName = _modelName,
                ModelClassName = _modelClassName,
                ModelFields = columns.Select(r => new { Name = r.Split('→')[0], Comment = r.Split('→')[1] }).ToArray()
            };
            var assembly = typeof(GeneratorCodeHelper).GetTypeInfo().Assembly;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, false))
                {
                    string file;
                    string result;
                    Template template;
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream("Infrastructure.CodeGen.CrudTemplete.View.cshtml.tpl"), Encoding.UTF8))
                    {
                        file = reader.ReadToEnd();
                        template = Template.Parse(file);
                        result = template.Render(Hash.FromAnonymousObject(obj));
                        ZipArchiveEntry entry = zip.CreateEntry(_modelClassName + ".cshtml");
                        using (StreamWriter entryStream = new StreamWriter(entry.Open()))
                        {
                            entryStream.Write(result);
                        }
                    }

                    using (var reader =new StreamReader(assembly.GetManifestResourceStream("Infrastructure.CodeGen.CrudTemplete.Respository.tpl"),Encoding.UTF8))
                    {
                        file = reader.ReadToEnd();
                        template = Template.Parse(file);
                        result = template.Render(Hash.FromAnonymousObject(obj));
                        ZipArchiveEntry entry1 = zip.CreateEntry(_modelClassName + "Respository.cs");
                        using (StreamWriter entryStream = new StreamWriter(entry1.Open()))
                        {
                            entryStream.Write(result);
                        }
                    }

                    using (var reader =new StreamReader(assembly.GetManifestResourceStream("Infrastructure.CodeGen.CrudTemplete.ViewModel.tpl"), Encoding.UTF8))
                    {
                        file = reader.ReadToEnd();
                        template = Template.Parse(file);
                        result = template.Render(Hash.FromAnonymousObject(obj));
                        ZipArchiveEntry entry2 = zip.CreateEntry(_modelClassName + "Vm.cs");
                        using (StreamWriter entryStream = new StreamWriter(entry2.Open()))
                        {
                            entryStream.Write(result);
                        }
                    }

                    using (var reader =new StreamReader(assembly.GetManifestResourceStream("Infrastructure.CodeGen.CrudTemplete.IRespository.tpl"),Encoding.UTF8))
                    {
                        file = reader.ReadToEnd();
                        template = Template.Parse(file);
                        result = template.Render(Hash.FromAnonymousObject(obj));
                        ZipArchiveEntry entry3 = zip.CreateEntry("I" + _modelClassName + "Respository.cs");
                        using (StreamWriter entryStream = new StreamWriter(entry3.Open()))
                        {
                            entryStream.Write(result);
                        }
                    }

                    using (var reader =new StreamReader(assembly.GetManifestResourceStream("Infrastructure.CodeGen.CrudTemplete.Controller.tpl"), Encoding.UTF8))
                    {
                        file = reader.ReadToEnd();
                        template = Template.Parse(file);
                        result = template.Render(Hash.FromAnonymousObject(obj));
                        ZipArchiveEntry entry4 = zip.CreateEntry(_modelClassName + "Controller.cs");
                        using (StreamWriter entryStream = new StreamWriter(entry4.Open()))
                        {
                            entryStream.Write(result);
                        }
                    }

                }
                data = ms.ToArray();
            }
            return data;
        }


    }
}
