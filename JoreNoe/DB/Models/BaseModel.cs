using System;
using System.ComponentModel;

namespace JoreNoe.DB.Models
{
    public class BaseModel<MID>
    {
        public BaseModel()
        {
            var converter = TypeDescriptor.GetConverter(typeof(MID));
            if (typeof(MID) is Guid)
                this.Id = (MID)converter.ConvertTo(Guid.NewGuid(), typeof(Guid));
        }

        /// <summary>
        /// 主键Id
        /// </summary>
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
