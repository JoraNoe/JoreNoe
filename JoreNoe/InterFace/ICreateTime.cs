using System;

namespace JoreNoe.InterFace
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public interface ICreateTime
    {
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 可空时间
    /// </summary>
    public interface INullCreateTIme
    {
        public DateTime? CreateTime { get; set; }
    }
}
