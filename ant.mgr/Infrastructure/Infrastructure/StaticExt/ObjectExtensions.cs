using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static class ObjectExtensions
    {



        /// <summary>
        /// 把Dictionary 加在string 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        public static string AppendDic(this string value, Dictionary<string, string> addition)
        {
            if (addition == null) return value;
            value += Environment.NewLine + "            [Tags]" + Environment.NewLine;
            foreach (KeyValuePair<string, string> item in addition)
            {
                value += "                 {Key:" + item.Key + ",Value:" + item.Value + "}" + Environment.NewLine;
            }
            return value;
        }

        

        /// <summary>
        /// 将流读取成字节组。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>字节组。</returns>
        public static byte[] ReadBytes(this Stream stream)
        {
            if (!stream.CanRead)
                throw new NotSupportedException(stream + "不支持读取。");

            Action trySeekBegin = () =>
            {
                if (!stream.CanSeek)
                    return;

                stream.Seek(0, SeekOrigin.Begin);
            };

            trySeekBegin();

            var list = new List<byte>(stream.CanSeek ? (stream.Length > int.MaxValue ? int.MaxValue : (int)stream.Length) : 300);

            int b;

            while ((b = stream.ReadByte()) != -1)
                list.Add((byte)b);

            trySeekBegin();

            return list.ToArray();
        }
      
        
       

    }
}
