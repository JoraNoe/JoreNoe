﻿using JoreNoe.DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace JoreNoe.DB.EntityFrameWork.Core.SqlServer
{
    public class Repository<MID, T> :IRepository<MID, T> where T : BaseModel<MID>, new()
    {
        /// <summary>
        /// 基类
        /// </summary>
        private DbContext Db { get; set; }

        public Repository()
        {
            this.Db = Register._Dbcontext;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Add(T t)
        {
            this.Db.Set<T>().Add(t);
            return t;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T t)
        {
            await this.Db.Set<T>().AddAsync(t);
             
            return t;
        }
        /// <summary>
        /// 添加同步
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<T> AddRange(IList<T> t)
        {
            this.Db.Set<T>().AddRange(t);
             
            return t.ToList();
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<IList<T>> AddRangeAsync(IList<T> t)
        {
            await this.Db.Set<T>().AddRangeAsync(t);
             
            return t;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public async Task<IList<T>> AllAsync()
        {
            var Result = await this.Db.Set<T>().Where(d => true).ToListAsync();
            return Result;
        }
        /// <summary>
        /// 删除同步
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T SoftDelete(MID Id)
        {

            var Re = new T();
            if (!this.Db.Set<T>().Any(d => d.Id.ToString() == Id.ToString()))
                return null;
            Re = this.GetSingle(Id).Result;

            if (Re == null)
            {
                return null;
            }
            Re.IsDelete = true;
            var Result = this.Db.Set<T>().Update(Re);
             
            return Result.Entity;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<T> DeleteAsync(MID Id)
        {
            var Result = this.Db.Set<T>().Remove(new T { Id = Id }); ;
             
            return Task.Run(() => { return Result.Entity; });
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRangeAsync(MID[] Id)
        {
            var Find = this.Db.Set<T>().Where(d => Id.Contains(d.Id));
            this.Db.Set<T>().RemoveRange(Find);
            return await Task.Run(() =>
            {
                return true;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<T> EditAsync(T t)
        {
            var Result = this.Db.Set<T>().Update(t);
             
            return Task.Run(() => { return Result.Entity; });
        }
        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<T> GetSingle(MID Id)
        {
            return await this.Db.Set<T>().AsTracking().SingleAsync(d => d.Id + string.Empty == Id + string.Empty && !d.IsDelete);
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
            var Page = Result.Skip(PageNum - 1 * PageSize).Take(PageNum * PageSize);
            return Page.ToList();
        }
        /// <summary>
        /// 同步获取数据
        /// </summary>
        /// <returns></returns>
        public List<T> All()
        {
            return this.Db.Set<T>().AsNoTracking().Where(d => true && !d.IsDelete).ToList();
        }
        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T Delete(MID Id)
        {
            var Re = new T();
            if (!this.Db.Set<T>().Any(d => d.Id + string.Empty == Id + string.Empty))
                return null;
            Re = this.GetSingle(Id).Result;
            this.Db.Set<T>().Remove(Re);
             
            return Re;
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
            return this.Db.Set<T>().Where(Func).ToList();

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
        /// 总数量
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public async Task<int> TotalAsync(Func<T, bool> Func = null)
        {
            return await this.Db.Set<T>().CountAsync();
        }

        #region 初始化
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="DB"></param>
        public void InitDb(DbContext DB)
        {
            this.Db = DB;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void SaveChange()
        {
            this.Db.SaveChanges();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.Db.Dispose();
        }
        #endregion

        #region 无保存 
        public T AddIngoreSave(T t)
        {
            this.Db.Set<T>().Add(t);
            return t;
        }

        public T EditIngoreSave(T t)
        {
            this.Db.Set<T>().Update(t);
            return t;
        }

        public T DeleteIngoreSave(MID Id)
        {
            var Entity = this.Db.Set<T>().SingleOrDefault(d => d.Id + string.Empty == Id + String.Empty);
            if (Entity != null)
                this.Db.Set<T>().Remove(Entity);
            return null;
        }

        /// 总数
        /// </summary>
        /// <param name="Func"></param>
        /// <returns></returns>
        public int Count(Func<T, bool> Func)
        {
            return this.Db.Set<T>().Count(Func);
        }
        public async Task<T> AddIngoreSaveAsync(T t)
        {
            await this.Db.Set<T>().AddAsync(t);
            return t;
        }
        #endregion
    }
}
