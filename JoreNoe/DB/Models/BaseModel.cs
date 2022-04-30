using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JoreNoe.DB.Models
{
    public class BaseModel<MID>
    {
        public BaseModel()
        {
            //var converter = TypeDescriptor.GetConverter(typeof(MID));
            //if (typeof(MID).IsAssignableFrom(typeof(Guid)))
            //    this.Id = (MID)converter.ConvertTo(Guid.NewGuid().ToString(), typeof(Guid));
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public MID Id { get; set; }

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
