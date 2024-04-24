using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.DB.EntityFrameWork.Core
{
    public interface IRepository<TKey, TEntity> where TEntity : class
    {
        Task<IList<TEntity>> FindAsync(Func<TEntity, bool> Func);
        Task<TEntity> AddAsync(TEntity t);
        Task<IList<TEntity>> AddRangeAsync(IList<TEntity> t);
        TEntity Edit(TEntity t);
        Task<TEntity> EditAsync(TEntity t);
        TEntity Update(TKey Id, Action<TEntity> t);
        Task<TEntity> UpdateAsync(TKey Id, Action<TEntity> t);
        TEntity Delete(TKey Id);
        Task<TEntity> DeleteAsync(TKey Id);
        TEntity Remove(TKey Id);
        bool DeleteRange(TKey[] Id);
        Task<TEntity> SingleAsync(TKey Id);
        TEntity Single(TKey Id);
        Task<IList<TEntity>> AllAsync();
        Task<IList<TEntity>> Page(int PageNum = 0, int PageSize = 10);
        Task<int> TotalAsync();
        List<TEntity> Find(Func<TEntity, bool> Func);
        TEntity Add(TEntity t);
        TEntity SoftDelete(TKey Id);
        IList<TEntity> AddRange(IList<TEntity> t);
        IList<TEntity> All();
        bool Exist(Func<TEntity, bool> Func);
        int Count(Func<TEntity, bool> Func = null);
        IList<TEntity> FindAsNoTracking(Func<TEntity, bool> Func);
        Task<IList<TEntity>> FindAsNoTracKingAsync(Func<TEntity, bool> Func);

        bool BulkInsert(IList<TEntity> Entitys);

        Task<bool> BulkInsertAsync(IList<TEntity> Entitys);

        bool BulkDelete(IList<TEntity> Entitys);

        Task<bool> BulkDeleteAsync(IList<TEntity> Entitys);

        bool BulkUpdate(IList<TEntity> Entitys);

        Task<bool> BulkUpdateAsync(IList<TEntity> Entitys);

    }
}
