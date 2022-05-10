using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe
{
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
