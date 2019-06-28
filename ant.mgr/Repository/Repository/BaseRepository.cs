using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AntData.ORM;
using AntData.ORM.Data;
using Configuration;
using DbModel;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Mapping;
using Repository.Interface;
using Newtonsoft.Json;

namespace Repository
{
    /// <summary>
    /// 基础仓库
    /// </summary>
    public class BaseRepository: IRepository
    {

        /// <summary>
        /// DB
        /// </summary>
        public MysqlDbContext<AntEntity> DB => DbModel.DbContext.DB;


        /// <summary>
        /// automapper
        /// </summary>
        /// <typeparam name="M1">原</typeparam>
        /// <typeparam name="M">目的对象</typeparam>
        /// <param name="source">原对象</param>
        /// <returns></returns>
        public M MapperTo<M1, M>(M1 source)
        {
            return Mapping.AutoMapper.MapperTo<M1, M>(source);
        }

        /// <summary>
        /// DB里面所有的Entity
        /// </summary>
        public AntEntity Entitys => DB.Tables;

        public int Execute(string sql, params DataParameter[] parameters)
        {
            return this.DB.Execute(sql, parameters);
        }
    }

    /// <summary>
    /// 基础泛型仓库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : BaseRepository,IRepository<T> where T : class
    {


        #region Impl


       

        /// <summary>
        /// 当前的Entity
        /// </summary>
        public IQueryable<T> Entity => DB.GetTable<T>();


        public int Save(T entity)
        {
            return this.DB.Insert(entity);
        }

        public long BatchSave(T[] entities)
        {
            return this.DB.BulkCopy(entities).RowsCopied;
        }

        public long InsertWithIdentity(T entity)
        {
            return (long)this.DB.InsertWithIdentity(entity);
        }




        public int Update(T entity)
        {
            return this.DB.Update(entity);
        }


        public int Delete(T entity)
        {
            return this.DB.Delete(entity);
        }

        public int BatchDelete(T[] entities)
        {
            return this.DB.Delete(entities);
        }



        public int Delete(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().Delete(exp);
        }

        public T FindSingle(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().FirstOrDefault(exp);
        }

        public bool IsExist(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().Any(exp);
        }

        public IEnumerable<T1> Query<T1>(string sql, params DataParameter[] parameters)
        {
            return this.DB.Query<T1>(sql, parameters);
        }

        public IEnumerable<T1> Query<T1>(T1 templete, string sql, params DataParameter[] parameters)
        {
            return this.DB.Query<T1>(templete, sql, parameters);
        }



        public T1 Execute<T1>(string sql, params DataParameter[] parameters)
        {
            return this.DB.Execute<T1>(sql, parameters);
        }

      
        #endregion


      
    }
}
