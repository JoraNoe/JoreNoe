using JoreNoe.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JoreNoe.DB.EntityFrameWork.Core.SqlServer
{
    public class Repository<TKey, TEntity> : IRepository<TKey, TEntity> where TEntity : BaseModel<TKey>, new()
    {
        /// <summary>
        /// 基类
        /// </summary>
        private DbContext EFContent { get; set; }

        public Repository(ICurrencyRegister Currency)
        {
            this.EFContent = Currency.Dbcontext;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public TEntity Add(TEntity Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException("实体为空");

            this.EFContent.Set<TEntity>().Add(Entity);
            return Entity;
        }

        /// <summary>
        /// 添加异步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<TEntity> AddAsync(TEntity Entity)
        {
            if (Entity == default || Entity == null)
                throw new ArgumentNullException("实体为空");
            await this.EFContent.Set<TEntity>().AddAsync(Entity);
            return Entity;
        }


        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IList<TEntity> AddRange(IList<TEntity> Entity)
        {
            if (Entity == default || Entity == null || Entity.Count == 0)
                throw new ArgumentNullException("实体为空");
            this.EFContent.Set<TEntity>().AddRange(Entity);
            return Entity.ToList();
        }

        /// <summary>
        /// 批量添加异步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> AddRangeAsync(IList<TEntity> Entity)
        {
            if (Entity == default || Entity == null || Entity.Count == 0)
                throw new ArgumentNullException("实体为空");
            await this.EFContent.Set<TEntity>().AddRangeAsync(Entity);
            return Entity;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TEntity>> AllAsync()
        {
            var Result = this.EFContent.Set<TEntity>().Where(d => true);
            if (Result == null)
                return new List<TEntity>();
            return await Result.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 删除同步
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TEntity SoftDelete(TKey Id)
        {
            if (!this.Exists(Id))
                return null;
            var Result = this.Update(Id, e => { e.IsDelete = true; });
            return Result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TEntity Delete(TKey Id)
        {
            if (!this.Exists(Id))
                return null;
            var Entity = this.Single(Id);
            if (Entity == null)
                return null;

            var Result = this.EFContent.Set<TEntity>().Remove(Entity);
            return Result.Entity;
        }

        public TEntity Remove(TKey Id)
        {
            return this.Delete(Id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteRange(TKey[] Ids)
        {
            var Find = this.EFContent.Set<TEntity>().Where(d => Ids.Contains(d.Id));
            if (Find == null)
                return false;
            this.EFContent.Set<TEntity>().RemoveRange(Find);
            return true;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public TEntity Edit(TEntity Entity)
        {
            if (!this.Exists(Entity.Id))
                return null;

            var Result = this.EFContent.Set<TEntity>().Update(Entity);
            return Result.Entity;
        }
        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<TEntity> SingleAsync(TKey Id)
        {
            if (!this.Exists(Id))
                return null;
            return await this.EFContent.Set<TEntity>().SingleOrDefaultAsync(d => d.Id + string.Empty == Id + string.Empty);
        }

        /// <summary>
        /// 同步获取单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TEntity Single(TKey Id)
        {
            if (!this.Exists(Id))
                return null;
            return this.EFContent.Set<TEntity>().SingleOrDefault(d => d.Id + string.Empty == Id + string.Empty);
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> Page(int PageNum = 0, int PageSize = 10)
        {
            var Result = await AllAsync();
            if (Result == null)
                return null;
            var Page = Result.Skip(PageNum - 1 * PageSize).Take(PageNum * PageSize);
            return Page.ToList();
        }
        /// <summary>
        /// 同步获取数据
        /// </summary>
        /// <returns></returns>
        public List<TEntity> AllNoTracking()
        {
            var Result = this.EFContent.Set<TEntity>().AsNoTracking();
            if (Result == null)
                return new List<TEntity>();
            return Result.ToList();
        }

        /// <summary>
        /// 自定义查询数据
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> FindAsync(Func<TEntity, bool> Func)
        {
            return await Task.Run(() =>
            {
                var Result = this.EFContent.Set<TEntity>().Where(Func);
                if (Result == null)
                    return new List<TEntity>();
                return Result.ToList();
            }).ConfigureAwait(false);

        }

        /// <summary>
        /// 自定义查询内容 同步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public List<TEntity> Find(Func<TEntity, bool> Func)
        {
            var Result = this.EFContent.Set<TEntity>().Where(Func);
            if (Result == null)
                return new List<TEntity>();
            return Result.ToList();

        }
        /// <summary>
        /// 查询是否存在 同步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public bool Exist(Func<TEntity, bool> Func)
        {
            var ExistsResult = this.EFContent.Set<TEntity>().Where(Func).FirstOrDefault();
            return ExistsResult == null ? false : true;
        }

        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Exists(TKey Id)
        {
            return this.EFContent.Set<TEntity>().SingleOrDefault(d => d.Id + string.Empty == Id + string.Empty) == null ? false : true;
        }

        /// <summary>
        /// 总数量
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<int> TotalAsync()
        {
            return await this.EFContent.Set<TEntity>().CountAsync();
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public IList<TEntity> FindAsNoTracking(Func<TEntity, bool> Func)
        {
            return this.EFContent.Set<TEntity>().AsNoTracking().Where(Func).ToList();
        }


        /// <summary>
        /// 查询异步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> FindAsNoTracKingAsync(Func<TEntity, bool> Func)
        {
            var Find = this.EFContent.Set<TEntity>().AsNoTracking().Where(Func).ToList();
            if (Find == null || Find.Count == 0)
                return null;
            return await Task.Run(() =>
            {
                return Find;
            }).ConfigureAwait(false);
        }

        public IList<TEntity> All()
        {
            return this.EFContent.Set<TEntity>().ToList();
        }

        public int Count(Func<TEntity, bool> Func = null)
        {
            if (Func == null)
                return this.EFContent.Set<TEntity>().Count();

            return this.EFContent.Set<TEntity>().Count(Func);
        }

        public async Task<TEntity> EditAsync(TEntity Entity)
        {
            if (Entity == null)
                throw new ArgumentNullException(nameof(Entity));
            this.EFContent.Set<TEntity>().Update(Entity);
            return await Task.Run(() =>
            {
                return Entity;
            }).ConfigureAwait(false);
        }


        public TEntity Update(TKey Id, Action<TEntity> ActionRever)
        {
            if (ActionRever == null)
                throw new ArgumentNullException(nameof(ActionRever));
            var Single = this.Single(Id);
            if (Single == null)
                return null;

            ActionRever(Single);
            this.EFContent.Set<TEntity>().Update(Single);
            return Single;
        }

        public async Task<TEntity> UpdateAsync(TKey Id, Action<TEntity> ActionRever)
        {
            if (ActionRever == null)
                throw new ArgumentNullException(nameof(ActionRever));
            var Single = await this.SingleAsync(Id).ConfigureAwait(false);
            if (Single == null)
                return null;

            ActionRever(Single);
            this.EFContent.Set<TEntity>().Update(Single);
            return Single;
        }

        public async Task<TEntity> DeleteAsync(TKey Id)
        {
            if (Id == null)
                return null;

            var Single = await this.SingleAsync(Id).ConfigureAwait(false);
            if (Single == null)
                return null;

            var RemoveInfo = this.EFContent.Remove(Single);
            return RemoveInfo.Entity;
        }

        public bool BulkInsert(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;

            this.EFContent.BulkInsert<TEntity>(Entitys);

            return true;
        }

        public async Task<bool> BulkInsertAsync(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;
            await this.EFContent.BulkInsertAsync<TEntity>(Entitys).ConfigureAwait(false);
            return true;
        }

        public bool BulkDelete(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;

            this.EFContent.BulkDelete<TEntity>(Entitys);

            return true;
        }

        public async Task<bool> BulkDeleteAsync(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;

            await this.EFContent.BulkDeleteAsync<TEntity>(Entitys).ConfigureAwait(false);

            return true;
        }

        public bool BulkUpdate(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;
            this.EFContent.BulkUpdate<TEntity>(Entitys);
            return true;
        }

        public async Task<bool> BulkUpdateAsync(IList<TEntity> Entitys)
        {
            if (Entitys == null || Entitys.Count == 0)
                return false;
            await this.EFContent.BulkUpdateAsync<TEntity>(Entitys).ConfigureAwait(false);

            return true;
        }
    }
}
