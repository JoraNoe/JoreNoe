using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.DB.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            this.Id = Guid.NewGuid();
            this.IsDelete = false;
            this.CreateTime = DateTime.Now;

        }

        /// <summary>
        /// 主键Id
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 是否删除 软删除
        /// </summary>
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
