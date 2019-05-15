using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mapping
{
    public class AutoMapper
    {
        public static MapperConfiguration Configuration { get; private set; }

        public static T MapperTo<T>(object source)
        {
            var _configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(source.GetType(), typeof(T));
            });
            return _configuration.CreateMapper().Map<T>(source);
        }
        
        public static T MapperTo<T1, T>(T1 source)
        {
            var _configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T1, T>() ;
            });
            return _configuration.CreateMapper().Map<T>(source);
        }

        public static T MapperToWithIgnore<T1, T>(T1 source, params string[] ignoreName)
        {
            var _configuration = new MapperConfiguration(cfg =>
            {
                var exp = cfg.CreateMap<T1, T>();
                foreach (var name in ignoreName)
                {
                    exp.ForMember(name, m => m.Ignore());
                }
            });
            return _configuration.CreateMapper().Map<T>(source);
        }

        public static List<T> MapperToList<T1, T>(List<T1> source)
        {
            var _configuration = new MapperConfiguration(cfg => cfg.CreateMap<T1, T>());
            return _configuration.CreateMapper().Map<List<T1>, List<T>>(source);
        }

        public static List<T> MapperToListDoAfter<T1, T>(List<T1> source, Action<List<T1>, List<T>> action)
        {
            var _configuration = new MapperConfiguration(cfg => cfg.CreateMap<T1, T>());
            return _configuration.CreateMapper().Map<List<T1>, List<T>>(source, ops =>
            {
                try
                {
                    ops.AfterMap(action);
                }
                catch
                {

                    //ignore
                }
            });
        }

        public void Execute(Assembly assembly)
        {
            Configuration = new MapperConfiguration(
                cfg =>
                {
                    var types = assembly.GetExportedTypes();
                    LoadStandardMappings(types, cfg);
                    LoadReverseMappings(types, cfg);
                    LoadCustomMappings(types, cfg);
                });
        }

        public void ExecuteByAssemblyName(params string[] assemblys)
        {
            if (assemblys.Length < 1)
            {
                throw new ArgumentException("assemblys");
            }

            var all = AppDomain.CurrentDomain.GetAssemblies();
            var modelAss = all.Where(assembly => assemblys.Contains(assembly.GetName().Name)).ToArray();
            if (!modelAss.Any())
            {
                return;
            }

            Configuration = new MapperConfiguration(
                       cfg =>
                       {
                           foreach (var assembly in modelAss)
                           {
                               var types = assembly.GetExportedTypes();
                               LoadStandardMappings(types, cfg);
                               LoadReverseMappings(types, cfg);
                               LoadCustomMappings(types, cfg);
                           }
                       });

        }

        private static void LoadStandardMappings(IEnumerable<Type> types, IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();

            foreach (var map in maps)
            {
                mapperConfiguration.CreateMap(map.Source, map.Destination);
            }
        }

        private static void LoadReverseMappings(IEnumerable<Type> types, IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select new
                        {
                            Destination = i.GetGenericArguments()[0],
                            Source = t
                        }).ToArray();

            foreach (var map in maps)
            {
                mapperConfiguration.CreateMap(map.Source, map.Destination);
            }
        }

        private static void LoadCustomMappings(IEnumerable<Type> types, IMapperConfigurationExpression mapperConfiguration)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IHaveCustomMappings).IsAssignableFrom(t) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(mapperConfiguration);
            }
        }
    }
}
