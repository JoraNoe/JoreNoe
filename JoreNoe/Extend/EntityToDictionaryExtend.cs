using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JoreNoe.Extend
{
    /// <summary>
    /// 实体转换为字典类
    /// </summary>
    public class EntityToDictionaryExtend
    {
        /// <summary>
        /// 实体转换为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="IgnoreFields">过滤的字段</param>
        /// <returns></returns>
        public static Dictionary<string, object> EntityToDictionary<T>(T entity, string[] IgnoreFields = null)
        {
            var dictionary = new Dictionary<string, object>();
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var key = property.Name;
                if (IgnoreFields != null && IgnoreFields.Contains(key)) continue;
                var value = property.GetValue(entity, null);
                dictionary[key] = value;
            }
            return dictionary;
        }

    }
}
