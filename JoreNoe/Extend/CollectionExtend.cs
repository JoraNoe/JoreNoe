using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JoreNoe.Extend
{
    /// <summary>
    /// 集合数据类型扩展
    /// </summary>
    public static class CollectionExtend
    {
        /// <summary>
        /// 数据集合使用Yield分组
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="DataSources">数据</param>
        /// <param name="BatchSize">分批数量</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<TSource>> YieldCollectionBatchs<TSource>(this ICollection<TSource> DataSources,int BatchSize)
        {
            var BatchResult = new Collection<TSource>();
            foreach (var item in DataSources)
            {
                BatchResult.Add(item);
                if (BatchResult.Count == BatchSize)
                {
                    yield return BatchResult;
                    BatchResult = new Collection<TSource>();
                }
            }
            if (BatchResult.Count > 0)
            {
                yield return BatchResult;
            }
        }
    }
}
