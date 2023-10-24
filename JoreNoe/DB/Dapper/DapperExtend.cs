using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.DB.Dapper
{
    public static class DapperExtend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, size - 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int size)
        {
            yield return source.Current;
            for (int i = 0; i < size && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }
    }
}
