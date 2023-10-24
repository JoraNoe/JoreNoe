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
        T Single(string ParamsValue, string ParamsKeyName = "Id", string[] ParamsColumns = null);

        /// <summary>
        /// 删除单条数据，物理删除
        /// </summary>
        /// <param name="ParamsValue"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        T Remove(string ParamsValue, string ParamsKeyName = "Id");

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="ParamsValues"></param>
        /// <param name="ParamsKeyName"></param>
        void Removes(IEnumerable<object> ParamsValues, string ParamsKeyName = "Id");

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ParamsValue"></param>
        /// <param name="Entity"></param>
        /// <param name="ParamsKeyName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        T Update(string ParamsValue, T Entity, string ParamsKeyName = "Id");

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<T> AddAsync<T>(T entity);

        /// <summary>
        /// 添加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add<T>(T entity);
    }
}
