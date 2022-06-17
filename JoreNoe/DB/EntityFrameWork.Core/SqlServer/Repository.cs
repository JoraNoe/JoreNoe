using JoreNoe.DB.Models;
using JoreNoe.DB.TEntityFrameWork.Core.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JoreNoe.DB.EntityFrameWork.Core.SqlServer
{
    public class Repository<MID, T> : IRepository<MID, T> where T : BaseModel<MID>, new()
    {

        // public class Repository<TEntity, TKey, TDbContext> : Repository<TEntity, TDbContext>, IRepository<TEntity, TKey> where TEntity : Entity<TKey> where TDbContext : EFContext

        /// <summary>
        /// 基类
        /// </summary>
        private DbContext Db { get; set; }

        public Repository(ICurrencyRegister Currency)
        {
            this.Db = Currency.Dbcontext;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Add(T t)
        {
            if (t == null)
                throw new ArgumentNullException("实体为空");

            this.Db.Set<T>().Add(t);
            return t;
        }

        /// <summary>
        /// 添加异步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T t)
        {
            if (t == default || t == null)
                throw new ArgumentNullException("实体为空");
            await this.Db.Set<T>().AddAsync(t);
            return t;
        }


        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IList<T> AddRange(IList<T> t)
        {
            if (t == default || t == null || t.Count == 0)
                throw new ArgumentNullException("实体为空");
            this.Db.Set<T>().AddRange(t);
            return t.ToList();
        }

        /// <summary>
        /// 批量添加异步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<IList<T>> AddRangeAsync(IList<T> t)
        {
            if (t == default || t == null || t.Count == 0)
                throw new ArgumentNullException("实体为空");
            await this.Db.Set<T>().AddRangeAsync(t);
            return t;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public async Task<IList<T>> AllAsync()
        {
            var Result = this.Db.Set<T>().Where(d => true);
            if (Result == null)
                return new List<T>();
            return await Result.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 删除同步
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T SoftDelete(MID Id)
        {
            if (!this.Exists(Id))
                return null;
            var Result = this.Single(Id);
            if (Result == null)
                return null;
            Result.IsDelete = true;
            this.Db.Set<T>().Update(Result);
            return Result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T Delete(MID Id)
        {
            if (!this.Exists(Id))
                return null;
            var Result = this.Db.Set<T>().Remove(new T { Id = Id });
            return Result.Entity;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteRange(MID[] Ids)
        {
            var Find = this.Db.Set<T>().Where(d => Ids.Contains(d.Id));
            if (Find == null)
                return false;
            this.Db.Set<T>().RemoveRange(Find);
            return true;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Edit(T t)
        {
            if (!this.Exists(t.Id))
                return null;

            var Result = this.Db.Set<T>().Update(t);
            return Result.Entity;
        }
        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<T> SingleAsync(MID Id)
        {
            if (!this.Exists(Id))
                return null;
            return await this.Db.Set<T>().SingleAsync(d => d.Id + string.Empty == Id + string.Empty && !d.IsDelete);
        }

        /// <summary>
        /// 同步获取单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T Single(MID Id)
        {
            if (!this.Exists(Id))
                return null;
            return this.Db.Set<T>().Single(d => d.Id + string.Empty == Id + string.Empty && !d.IsDelete);
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<IList<T>> Page(int PageNum = 0, int PageSize = 10)
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
        public List<T> AllNoTracking()
        {
            var Result = this.Db.Set<T>().AsNoTracking().Where(d => true && !d.IsDelete);
            if (Result == null)
                return new List<T>();
            return Result.ToList();
        }

        /// <summary>
        /// 自定义查询数据
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<IList<T>> FindAsync(Func<T, bool> Func)
        {
            return await Task.Run(() =>
            {
                var Result = this.Db.Set<T>().Where(Func);
                if (Result == null)
                    return new List<T>();
                return Result.ToList();
            }).ConfigureAwait(false);

        }

        /// <summary>
        /// 自定义查询内容 同步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public List<T> Find(Func<T, bool> Func)
        {
            var Result = this.Db.Set<T>().Where(Func);
            if (Result == null)
                return new List<T>();
            return Result.ToList();

        }
        /// <summary>
        /// 查询是否存在 同步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public bool Exist(Func<T, bool> Func)
        {
            var ExistsResult = this.Db.Set<T>().Where(Func).FirstOrDefault();
            return ExistsResult == null ? false : true;
        }

        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Exists(MID Id)
        {
            return this.Db.Set<T>().Find(Id) == null ? false : true;
        }

        /// <summary>
        /// 总数量
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<int> TotalAsync(Func<T, bool> Func = null)
        {
            return await this.Db.Set<T>().CountAsync();
        }


        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public IList<T> FindAsNoTracking(Func<T, bool> Func)
        {
            return this.Db.Set<T>().AsNoTracking().Where(Func).ToList();
        }


        /// <summary>
        /// 查询异步
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<IList<T>> FindAsNoTracKingAsync(Func<T, bool> Func)
        {
            var Find = this.Db.Set<T>().AsNoTracking().Where(Func).ToList();
            if (Find == null || Find.Count == 0)
                return null;
            return await Task.Run(() =>
            {
                return Find;
            }).ConfigureAwait(false);
        }

        public IList<T> All()
        {
            return this.Db.Set<T>().Where(d=>true).ToList();
        }

        public int Count(Func<T, bool> Func)
        {
            return this.Db.Set<T>().Count(Func);
        }
    }
}
