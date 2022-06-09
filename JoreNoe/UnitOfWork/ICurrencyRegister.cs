using Microsoft.EntityFrameworkCore;

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
