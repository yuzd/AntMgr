using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Configuration
{
    public static class ConfigHelper
    {
        private static readonly IConfigurationRoot Configuration;
        static ConfigHelper()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        }
        public static T GetConfig<T>(string key, T defaultValue)
        {
            try
            {
                var result = Configuration[key];
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception)
            {
                if (defaultValue != null)
                {
                    return defaultValue;
                }
                return default(T);
            }
        }

        public static T GetConfig<T>(string key)
        {
            try
            {
                var result = Configuration[key];
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception)
            {
                throw new Exception(string.Format("没有在配置文件中的appSettings中找到{0}的配置，请检查配置文件配置！", key));
            }
        }
    }
}
