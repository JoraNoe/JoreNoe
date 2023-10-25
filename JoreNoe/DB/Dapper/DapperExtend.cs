using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// 判断泛型是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        /// <summary>
        /// 获取数据字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string GetEntityFiledParams<T>(T Data)
        {
            Type type = Data.GetType();
            PropertyInfo[] properties = type.GetProperties();
            // 使用线程安全的集合来存储处理结果
            var resultBag = new List<object>();

            foreach (PropertyInfo property in properties)
            {
                //string propertyName = property.Name;
                var propertyValue = property.GetValue(Data, null); // 获取属性值
                // 根据属性的类型判断是否需要添加单引号
                var processedValue = (propertyValue is string || propertyValue is DateTime) ? $"'{propertyValue}'" : propertyValue;
                resultBag.Add(processedValue);
            }

            // 合并结果
            object[] results = resultBag.ToArray();
            string result = string.Join(",", results);
            return result;
        }

    }
}
