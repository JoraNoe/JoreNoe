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
    }
}
