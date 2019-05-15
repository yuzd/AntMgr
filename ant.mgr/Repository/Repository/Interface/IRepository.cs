//-----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using AntData.ORM.Data;
using DbModel;

namespace Repository.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public interface IRepository
    {
        /// <summary>
        /// 当前Query集合
        /// </summary>
        AntEntity Entitys { get; }


        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int Execute(string sql, params DataParameter[] parameters);
    }

   
    public interface IRepository<T> : IRepository where T : class
    {

       

        /// <summary>
        /// 当前Query
        /// </summary>
        IQueryable<T> Entity { get; }

        /// <summary>
        /// 保存 单条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Save(T entity);

        /// <summary>
        /// 保存 多条
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        long BatchSave(T[] entities);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
#pragma warning disable 693
        long InsertWithIdentity(T entity);
#pragma warning restore 693


        /// <summary>
        /// 更新 一条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(T entity);

        /// <summary>
        /// 删除一条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        int Delete(T entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        int Delete(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 搜索一条
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        T FindSingle(Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        bool IsExist(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 查询 返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
#pragma warning disable 693
        IEnumerable<T> Query<T>(string sql, params DataParameter[] parameters);
#pragma warning restore 693

        /// <summary>
        /// 查询 返回集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templete"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
#pragma warning disable 693
        IEnumerable<T> Query<T>(T templete, string sql, params DataParameter[] parameters);
#pragma warning restore 693

 

        /// <summary>
        /// 查询 返回单条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
#pragma warning disable 693
        T Execute<T>(string sql, params DataParameter[] parameters);
#pragma warning restore 693



        
    }
}