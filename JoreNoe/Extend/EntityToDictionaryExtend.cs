using JoreNoe.DB.Dapper.JoreNoeDapperAttribute;
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
                var value = property.GetValue(entity);
                    dictionary[key] = value;
            }
            return dictionary;
        }

        public static Dictionary<string, object> ObjectToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();

            // 获取对象的属性并添加到字典中
            foreach (var property in obj.GetType().GetProperties())
            {
                dictionary[property.Name] = property.GetValue(obj);
            }

            return dictionary;
        }

        /// <summary>
        /// 将实体转换为插入SQL参数列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IgnoreFields"></param>
        /// <returns></returns>
        public static Tuple<string, string> EntityToSQLParams<T>(string[] IgnoreFields = null)
        {
            // 使用反射获取实体的属性名称
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> NoLatterParams = new List<string>();
            List<string> LatterParams = new List<string>();
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(InsertIgnoreAutoIncrementAttribute)))
                    continue;

                if (IgnoreFields != null && IgnoreFields.Contains(property.Name)) continue;
                NoLatterParams.Add(property.Name);
                LatterParams.Add(string.Concat("@", property.Name));
            }
            // 将属性名称连接成一个逗号分隔的字符串
            string columnNamesNoLatter = string.Join(",", NoLatterParams.ToArray());
            string columnNamesLatter = string.Join(",", LatterParams.ToArray());
            return new Tuple<string, string>(columnNamesNoLatter, columnNamesLatter);
        }

        /// <summary>
        /// 将实体转换为插入SQL参数列 带 @ 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IgnoreFields"></param>
        /// <returns></returns>
        public static string EntityToSQLParamsLatter<T>(string[] IgnoreFields = null)
        {
            // 使用反射获取实体的属性名称
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> list = new List<string>();
            foreach (var property in properties)
            {
                if (IgnoreFields != null && IgnoreFields.Contains(property.Name)) continue;
                list.Add(string.Concat("@", property.Name));
            }
            // 将属性名称连接成一个逗号分隔的字符串
            string columnNames = string.Join(",", list.ToArray());
            return columnNames;
        }

    }
}
