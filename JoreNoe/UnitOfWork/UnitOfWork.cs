using JoreNoe.DB.EntityFrameWork.Core.SqlServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        /// <summary>
        /// DB上下文
        /// </summary>
        public DbContext Dbcontext { get; set; }

        public UnitOfWork()
        {
            this.Dbcontext = Register._Dbcontext;
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        public void Commit()
        {
            this.Dbcontext.SaveChanges();
        }

        public void Dispose()
        {
            this.Dbcontext.Dispose();
        }
    }
}
