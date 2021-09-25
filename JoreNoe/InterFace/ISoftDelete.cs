using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.InterFace
{
    /// <summary>
    /// 删除接口
    /// </summary>
    public interface ISoftDelete
    {
        bool IsDelete { get; set; }
    }
}
