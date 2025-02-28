using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        Task<T> SingleAsync<TKey>(TKey ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null);
        Task<T> SingleAsync(Expression<Func<T, bool>> ExPression);
        T Single(Expression<Func<T, bool>> ExPression);
        T SingleSQL(string SQL);
        Task<T> SingleSQLAsync(string SQL);
        T SingleSQL(string SQL, object Params);
        Task<T> SingleSQLAsync(string SQL, object Params);

        /// <summary>
        /// 删除单条数据 物理删除
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="ParamsValue">主键值</param>
        /// <param name="ParamsKeyName">主键名称</param>
        /// <returns></returns>
        T Remove<TKey>(TKey ParamsValue, string ParamsKeyName = "Id");
        T Remove(Expression<Func<T, bool>> ExPression);
        void Removes<TKey>(TKey[] ParamsValues, string ParamsKeyName = "Id");
        T SoftRemove<TKey>(TKey ParamsValues, string ParamsKeyName = "Id", string SoftKeyName = "IsDelete", bool? SoftKeyValue = null);

        /// <summary>
        /// 查询是否存在数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ParamsValues"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        bool IsExists<TKey>(TKey ParamsValues, string ParamsKeyName = "Id");
        Task<bool> IsExistsAsync<TKey>(TKey ParamsValues, string ParamsKeyName = "Id");
        bool IsExists(Expression<Func<T, bool>> ExPression);
        Task<bool> IsExistsAsync(Expression<Func<T, bool>> ExPression);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="data"></param>
        void BulkInsert(IEnumerable<T> data);
        Task BulkInsertAsync(IEnumerable<T> data);
        void BulkInsertTransaction(IEnumerable<T> data);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="ParamsValue">主键值</param>
        /// <param name="Entity">修改的实体</param>
        /// <param name="ParamsKeyName">主键名称</param>
        /// <returns></returns>
        T Update<TKey>(TKey ParamsValue, object Entity, string ParamsKeyName = "Id");
        T Update<TKey>(TKey ParamsValue, Action<T> Entity, string ParamsKeyName = "Id");
        T Update<TKey>(TKey ParamsValue, T Entity, string ParamsKeyName = "Id");
        T Update<TKey>(TKey ParamsValue, Func<T, T> Entity, string ParamsKeyName = "Id");

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<T> AddAsync(T entity, string[] IgnoreFailds = null);
        T Add(T entity, string[] IgnoreFailds = null);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="SQLExcute">SQL 语句</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<T> Find(string SQLExcute);
        IEnumerable<T> Find(string SQLExcute, object Params);
        Task<IEnumerable<T>> FindAsync(string SQLExcute, object Params);
        IEnumerable<T> Find(Expression<Func<T, bool>> Predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> Predicate);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="SQLExcute"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        int Excute(string SQLExcute, object Params);
        int Excute(string SQLExcute);

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        bool CreateTable(T Entity);
        void PublishHistory(string LogContext, string ResultContext, string ContextType, string UserName = "SystemCreate", string TableName = "BaseHistory");
        Task TestMUlit(IEnumerable<T> D);

        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        int Count();
        int Count(Expression<Func<T, bool>> Predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> Predicate);
        int Count(string SQL);
    }
}
