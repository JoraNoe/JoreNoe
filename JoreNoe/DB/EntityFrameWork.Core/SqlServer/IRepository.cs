
using JoreNoe.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.DB.TEntityFrameWork.Core.SqlServer
{
    public interface IRepository<TKey, TEntity> where TEntity : Entity<TKey>
    {    
        #region 异步数据
        /// <summary>
        /// 自定义查询内容
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        Task<IList<TEntity>> FindAsync(Func<TEntity, bool> Func);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity t);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<IList<TEntity>> AddRangeAsync(IList<TEntity> t);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        TEntity Edit(TEntity t);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity Delete(TKey Id);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool DeleteRange(TKey[] Id);
        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(TKey Id);

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity Single(TKey Id);

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> AllAsync();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        Task<IList<TEntity>> Page(int PageNum = 0, int PageSize = 10);

        /// <summary>
        /// 总数量
        /// </summary>
        /// <returns></returns>
        Task<int> TotalAsync(Func<TEntity, bool> Func = null);

        #endregion
        #region 同步 数据
        /// <summary>
        /// 自定义查询内容
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        List<TEntity> Find(Func<TEntity, bool> Func);
        /// <summary>
        /// 添加同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        TEntity Add(TEntity t);

        /// <summary>
        /// 移除 同步 软删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity SoftDelete(TKey Id);
        /// <summary>
        /// 批量添加同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        List<TEntity> AddRange(IList<TEntity> t);
        /// <summary>
        /// 全部数据同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        List<TEntity> All();

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        bool Exist(Func<TEntity, bool> Func);

        /// <summary>
        /// 总数
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        int Count(Func<TEntity, bool> Func);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        IList<TEntity> FindAsNoTracking(Func<TEntity, bool> Func);

        /// <summary>
        /// 查询异步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        Task<IList<TEntity>> FindAsNoTracKingAsync(Func<TEntity, bool> Func);

        #endregion
    }
}
