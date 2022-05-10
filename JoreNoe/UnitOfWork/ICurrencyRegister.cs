using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe
{
    /// <summary>
    /// 通用注册接口
    /// </summary>
    public interface ICurrencyRegister
    {
        public DbContext Dbcontext { get; set; }
    }
}
