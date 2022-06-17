
using JoreNoe.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.DB.TEntityFrameWork.Core.SqlServer
{
    public interface IRepository<TKey, TEntity> where TEntity : class
    {
        Task<IList<TEntity>> FindAsync(Func<TEntity, bool> Func);
        Task<TEntity> AddAsync(TEntity t);
        Task<IList<TEntity>> AddRangeAsync(IList<TEntity> t);
        TEntity Edit(TEntity t);
        TEntity Delete(TKey Id);
        bool DeleteRange(TKey[] Id);
        Task<TEntity> SingleAsync(TKey Id);
        TEntity Single(TKey Id);
        Task<IList<TEntity>> AllAsync();
        Task<IList<TEntity>> Page(int PageNum = 0, int PageSize = 10);
        Task<int> TotalAsync(Func<TEntity, bool> Func = null);
        List<TEntity> Find(Func<TEntity, bool> Func);
        TEntity Add(TEntity t);
        TEntity SoftDelete(TKey Id);
        IList<TEntity> AddRange(IList<TEntity> t);
        IList<TEntity> All();
        bool Exist(Func<TEntity, bool> Func);
        int Count(Func<TEntity, bool> Func = null);
        IList<TEntity> FindAsNoTracking(Func<TEntity, bool> Func);
        Task<IList<TEntity>> FindAsNoTracKingAsync(Func<TEntity, bool> Func);
    }
}
