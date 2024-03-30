using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;

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

        /// <summary>
        /// 获取批次数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetBatchData<T>(this IEnumerable<T> data, long BatchCount)
        {
            return data
            .Select((value, index) => new { Value = value, Index = index })
            .GroupBy(x => x.Index / BatchCount)
            .Select(g => g.Select(x => x.Value).ToList());
        }

        public static string GetColumnName(PropertyInfo property)
        {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttr != null ? columnAttr.Name : property.Name;
        }

        public static string GetColumnType(PropertyInfo property)
        {
            var type = property.PropertyType;

            if (type == typeof(int))
            {
                StringBuilder Str = new StringBuilder();
                var cloumnAttr =  property.GetCustomAttribute<ColumnAutoIncrementAttribute>();
                if (cloumnAttr == null)
                    Str.Append("INT ");
                else Str.Append( "INT auto_increment ");
                var cloumnAttr1 = property.GetCustomAttribute<ColumnPrimaryKeyAttribute>();
                if (cloumnAttr1 != null)
                    Str.Append(" Primary Key ");
                return Str.ToString();
            }
            else if (type == typeof(string))
            {
                var columnAttr = property.GetCustomAttribute<ColumnLengthAttribute>();
                if (columnAttr != null)
                    return $"VARCHAR({columnAttr.Length})";
                else
                    return $"VARCHAR(255)";
            }
            else if(type == typeof(bool))
            {
                return $" TINYINT(1) ";
            }
            else if(type == typeof(DateTime))
            {
                return $" DATETIME ";
            }
            else if (type == typeof(Guid))
            {
                return $" UNIQUEIDENTIFIER ";
            }

            throw new ArgumentException($"Unsupported type: {type.Name}");
        }

        public static bool IsNullable(PropertyInfo property)
        {
            return Nullable.GetUnderlyingType(property.PropertyType) != null ||
                   property.GetCustomAttribute<RequiredAttribute>() == null;
        }

    }
}
