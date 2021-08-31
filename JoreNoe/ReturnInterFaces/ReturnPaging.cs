using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.CommonInterFaces
{
    /// <summary>
    /// 分页
    /// </summary>
    public class ReturnPaging<T>
    {
        public ReturnPaging(IList<T> Item,int Total = 0)
        {
            this.Total = Total;
            this.Item = Item;
        }

        public ReturnPaging(int PageNum, int PageSize, int Total)
        {
            this.PageNum = PageNum;
            this.PageSize = PageSize;
            this.Total = Total;
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 项
        /// </summary>
        public IList<T> Item { get; set; }
    }
}
