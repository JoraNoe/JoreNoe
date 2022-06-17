using Microsoft.EntityFrameworkCore;
using System;

namespace JoreNoe
{
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
}
