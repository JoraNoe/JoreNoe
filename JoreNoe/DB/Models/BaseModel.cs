using System;
using System.ComponentModel.DataAnnotations;

namespace JoreNoe.DB.Models
{
    public class BaseModel<TKey>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public TKey Id { get; set; }

        /// <summary>
        /// 是否删除 软删除
        /// </summary>
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
    }
}
