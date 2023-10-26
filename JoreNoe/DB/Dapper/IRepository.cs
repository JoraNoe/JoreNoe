using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.DB.Dapper
{
    public interface IRepository<T>
    {
        /// <summary>
        /// 尽量不要使用,全表检索
        /// </summary>
        /// <param name="ParamsColumns">输出列</param>
        /// <returns></returns>
        IEnumerable<T> All(string[] ParamsColumns = null);

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="ParamsValue">一般是主键ID </param>
        /// <param name="ParamsKeyName">主键名称 默认为 Id </param>
        /// <param name="ParamsColumns">输出的列名,默认为 * 全部</param>
        /// <returns></returns>
        T Single<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null);

        /// <summary>
        /// 删除单条数据 物理删除
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="ParamsValue">主键值</param>
        /// <param name="ParamsKeyName">主键名称</param>
        /// <returns></returns>
        T Remove<TKey>(TKey ParamsValue, string ParamsKeyName = "Id");

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="ParamsValues">主键值</param>
        /// <param name="ParamsKeyName">主键名称</param>
        void Removes<TKey>(TKey[] ParamsValues, string ParamsKeyName = "Id");

        /// <summary>
        /// 软删除 数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValues">软删除Key</param>
        /// <param name="ParamsKeyName">软删除Keyname</param>
        /// <param name="SoftKeyName">软删除字段</param>
        /// <param name="SoftKeyValue">软删除数据</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        T SoftRemove<TKey>(TKey ParamsValues, string ParamsKeyName = "Id", string SoftKeyName = "IsDelete",bool? SoftKeyValue = null);

        /// <summary>
        /// 查询是否存在数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValues"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        bool IsExists<TKey>(TKey ParamsValues, string ParamsKeyName = "Id");

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="data"></param>
        void BulkInsert(IEnumerable<T> data);

        /// <summary>
        /// 批量插入数据异步
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task BulkInsertAsync(IEnumerable<T> data);

        /// <summary>
        /// 批量插入数据事务
        /// </summary>
        /// <param name="data"></param>
        void BulkInsertTransaction(IEnumerable<T> data);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="ParamsValue">主键值</param>
        /// <param name="Entity">修改的实体</param>
        /// <param name="ParamsKeyName">主键名称</param>
        /// <returns></returns>
        T Update<TKey>(TKey ParamsValue, T Entity, string ParamsKeyName = "Id");

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns></returns>
        T Add(T entity);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="SQLExcute">SQL 语句</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<T> Find(string SQLExcute);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="SQLExcute">执行SQL</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<T> Find(string SQLExcute, object Params);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="SQLExcute"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        int Excute(string SQLExcute, object Params);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="SQLExcute">执行SQL</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        int Excute(string SQLExcute);
    }
}
