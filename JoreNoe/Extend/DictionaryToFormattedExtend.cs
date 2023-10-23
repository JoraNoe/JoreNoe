using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoreNoe.Extend
{
    public class DictionaryToFormattedExtend
    {
        /// <summary>
        /// 将字典转换成SQL参数
        /// </summary>
        /// <param name="dictionary">字典数据</param>
        /// <param name="IgnoreKeys">忽略的字段</param>
        /// <returns></returns>
        public static string DictionaryToFormattedSQL(Dictionary<string, object> dictionary, string[] IgnoreKeys = null)
        {
            var formattedString = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                if (IgnoreKeys != null && IgnoreKeys.Contains(kvp.Key)) continue;
                formattedString.Append($"{kvp.Key} = @{kvp.Key}, ");
            }

            // Remove the trailing comma and space
            if (formattedString.Length > 2)
            {
                formattedString.Remove(formattedString.Length - 2, 2);
            }

            return formattedString.ToString();
        }
    }
}
