using Microsoft.EntityFrameworkCore;
using System;

namespace JoreNoe.DB.EntityFrameWork.Core
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        void Commit();
    }

    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// DB上下文
        /// </summary>
        public DbContext Dbcontext { get; set; }

        public UnitOfWork(ICurrencyRegister CurrentyRegister)
        {
            this.Dbcontext = CurrentyRegister.Dbcontext;
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        public void Commit()
        {
            this.Dbcontext.SaveChanges();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.Dbcontext.Dispose();
        }
    }

    /// <summary>
    /// 通用注册接口
    /// </summary>
    public interface ICurrencyRegister
    {
        public DbContext Dbcontext { get; set; }
    }

    /// <summary>
    /// 基类
    /// </summary>
    public interface IBaseRepository
    {
        void SaveChanges();
    }

    /// <summary>
    /// 基类实现
    /// </summary>
    public class BaseRepository : IBaseRepository
    {
        /// <summary>
        /// 单元
        /// </summary>
        protected readonly IUnitOfWork UnitOfWork;

        public BaseRepository(IUnitOfWork UnitOfWork)
        {
            this.UnitOfWork = UnitOfWork;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void SaveChanges()
        {
            this.UnitOfWork.Commit();
        }
    }
}
