using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DbModel;

namespace Mapping
{
    public static class QueryableExtensions
    {
        public static IQueryable<TDestination> MappperTo<TDestination>(this IQueryable source, params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return source.ProjectTo(AutoMapper.Configuration, membersToExpand);
        }

        public static List<TDestination> MappperList<TDestination>(this IEnumerable<LinqToDBEntity> source)
        {
            return AutoMapper.Configuration.CreateMapper().Map<IEnumerable<LinqToDBEntity>, List<TDestination>>(source);
        }

        public static TDestination MappperTo<TDestination>(this LinqToDBEntity source)
        {
            return AutoMapper.Configuration.CreateMapper().Map<LinqToDBEntity, TDestination>(source);
        }

        public static TDestination MappperTo<TDestination>(this LinqToDBEntity source, Action<IMappingOperationOptions> options)
        {
            return AutoMapper.Configuration.CreateMapper().Map<LinqToDBEntity, TDestination>(source, options);
        }
    }
}
