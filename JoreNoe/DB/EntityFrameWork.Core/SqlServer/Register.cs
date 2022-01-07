using JoreNoe.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.DB.EntityFrameWork.Core.SqlServer
{
    /// <summary>
    /// 注册服务
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// EntityFrameCore 上下文
        /// </summary>
        public static DbContext _Dbcontext { get; set; }

        /// <summary>
        /// 设置上下文方法
        /// </summary>
        /// <param name="DB"></param>
        public static void SetInitDbContext(DbContext DB)
        {
            _Dbcontext = DB;
        }

        /// <summary>
        /// 服务
        /// </summary>
        /// <param name="Services"></param>
        public static void AddJoreNoeEntityFrameworkSQLServer(IServiceCollection Services)
        {
            _ = Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}

