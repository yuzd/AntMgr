using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid.Tags;
using HtmlAgilityPack;

namespace Infrastructure.Web
{
    public static class HtmlParse
    {
        /// <summary>
        /// 解析Html 获取 指定name的attribute 和对应的 value
        /// </summary>
        /// <param name="body"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="className">默认找input</param>
        /// <returns></returns>
        public static Dictionary<string,string> GetValueAndNameByClass(this string body,string name,string value,string className)
        {
            var result = new Dictionary<string,string>();
            if (string.IsNullOrEmpty(body))
            {
                return result;
            }
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);

            var htmlBody = htmlDoc.DocumentNode.SelectNodes($"//*[contains(@class,'{className}')]");
            if (htmlBody == null) return result;
            foreach (var item in htmlBody)
            {
                var namevalueDefault = item.Attributes.FirstOrDefault(r => r.Name.Equals(name));
                if (namevalueDefault != null)
                {
                    var valueDefault = item.Attributes.FirstOrDefault(r => r.Name.Equals(value));
                    if (valueDefault != null)
                    {
                        if (result.ContainsKey(namevalueDefault.Value))
                        {
                            result[namevalueDefault.Value] = valueDefault.Value;
                        }
                        else
                        {
                            result.Add(namevalueDefault.Value, valueDefault.Value);
                        }
                    }
                }
            }
            return result;
        }

    }
}
