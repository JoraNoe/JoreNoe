using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.Extend
{
    /// <summary>
    /// Key Value 类型数据 转换
    /// </summary>
    public static class KeyValueExtend
    {
        /// <summary>
        /// 可用枚举类型 默认 1
        /// 类型1：KeyValue
        /// </summary>
        public enum AvailableType { KeyValue = 1, TrueOrFalse = 2, OnOrOff = 3, EnableOrDisable = 4 }


        public static IList<KeyValuePair<int,string>> StringToKeyValue(this string Value,char Letter)
        {
            if(string.IsNullOrEmpty())
        }

        /// <summary>
        /// 特定转换类型 boolean
        /// </summary>
        public static string BooleanToString(this bool Value, AvailableType Type = AvailableType.IsOrDeny)
        {
            string Result;
            switch (Type)
            {
                case AvailableType.IsOrDeny:
                    Result = Value ? "是" : "否";
                    break;
                case AvailableType.TrueOrFalse:
                    Result = Value ? "真" : "假";
                    break;
                case AvailableType.OnOrOff:
                    Result = Value ? "开" : "关";
                    break;
                case AvailableType.EnableOrDisable:
                    Result = Value ? "启用" : "关闭";
                    break;
                default:
                    Result = "未知";
                    break;
            }
            return Result;
        }
    }
}
