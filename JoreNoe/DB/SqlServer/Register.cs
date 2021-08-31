using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.DB.SqlServer
{
    public static class Register
    {
        public static DbContext _Dbcontext { get; set; }

        public static void SetInitDbContext(DbContext DB)
        {
            _Dbcontext = DB;
        }
    }
}
