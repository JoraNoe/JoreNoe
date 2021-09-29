
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.DB.EntityFrameWork.Core.SqlServer
{
    public interface IRepository<T> where T : class
    {
        #region 异步数据
        /// <summary>
        /// 自定义查询内容
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        Task<IList<T>> FindAsync(Func<T, bool> Func);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> AddAsync(T t);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<IList<T>> AddRangeAsync(IList<T> t);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> EditAsync(T t);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> DeleteAsync(Guid Id);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> DeleteRangeAsync(Guid Id);
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetSingle(Guid Id);
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> AllAsync();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        Task<IList<T>> Page(int PageNum = 0, int PageSize = 10);

        /// <summary>
        /// 总数量
        /// </summary>
        /// <returns></returns>
        Task<int> TotalAsync(Func<T, bool> Func = null);

        #endregion
        #region 同步 数据
        /// <summary>
        /// 自定义查询内容
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        List<T> Find(Func<T, bool> Func);
        /// <summary>
        /// 添加同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        T Add(T t);

        /// <summary>
        /// 移除 同步 软删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T SoftDelete(Guid Id);
        /// <summary>
        /// 批量添加同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        List<T> AddRange(IList<T> t);
        /// <summary>
        /// 全部数据同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        List<T> All();
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T Delete(Guid Id);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        bool Exist(Func<T, bool> Func);

        /// <summary>
        /// 总数
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        int Count(Func<T,bool> Func);
        #endregion
        #region 无保存
        /// <summary>
        /// 添加无保存
        /// </summary>
        /// <returns></returns>
        T AddIngoreSave(T t);

        /// <summary>
        /// 修改无保存
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        T EditIngoreSave(T t);

        /// <summary>
        /// 删除无保存
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T DeleteIngoreSave(Guid Id);

        /// <summary>
        /// 添加无保存异步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<T> AddIngoreSaveAsync(T t);

        #endregion
        #region 初始化
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="DB"></param>
        void InitDb(DbContext DB);
        #endregion
    }
}
