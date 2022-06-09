using System.Collections.Generic;

namespace JoreNoe.CommonInterFaces
{
    /// <summary>
    /// 分页
    /// </summary>
    public class ReturnPaging<T>
    {
        public ReturnPaging(int PageNum = 1, int PageSize = 10, int Total = 0)
        {
            this.PageNum = PageNum;
            this.PageSize = PageSize;
            this.Total = Total;
        }

        /// <summary>
        /// 计算分页
        /// </summary>
        public void CalculatePaging()
        {
            if (this.Total != 0 && this.PageSize != 0)
            {
                string Count = ((float)this.Total / this.PageSize).ToString();
                int UseCount;
                if (Count.Contains("."))
                {
                    UseCount = int.Parse(Count.Split(".")[0]) + 1;
                }
                else UseCount = int.Parse(Count);
                this.PageCount = UseCount;
                this.Start = ((this.PageNum == 0 ? 1 : this.PageNum) - 1) * this.PageSize;
                this.End = PageSize;
            }
        }

        /// <summary>
        /// 开始数量
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束数量
        /// </summary>
        public int End { get; set; }

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
        /// 页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 项
        /// </summary>
        public IList<T> PagingItems { get; set; }
    }
}
