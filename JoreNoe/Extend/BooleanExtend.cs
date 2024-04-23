using System;
using System.Linq;

namespace JoreNoe.Extend
{
    /// <summary>
    /// Boolean扩展类
    /// </summary>
    public static class BooleanExtend
    {
        /// <summary>
        /// 可用枚举类型 默认 1
        /// 类型1：IsOrDeny 是 否
        /// 类型2：TrueOrFalse 真 假
        /// 类型3：OnOrOff 开 关 
        /// 类型4：EnableOrDisable 启用 关闭
        /// </summary>
        public enum AvailableType 
        {
            /// <summary>
            /// 是 否
            /// </summary>
            IsOrDeny = 1,
            /// <summary>
            /// 真 假
            /// </summary>
            TrueOrFalse = 2,
            /// <summary>
            /// 开 关 
            /// </summary>
            OnOrOff = 3,
            /// <summary>
            /// 启用 关闭
            /// </summary>
            EnableOrDisable = 4
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

        /// <summary>
        /// 自定义转换类型 boolean
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Letter">自定义参数，以逗号分割 第一个为True，第二个为false</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string BooleanToString(this bool Value, string Letter)
        {
            if (string.IsNullOrWhiteSpace(Letter))
                throw new ArgumentNullException("自定义标识为空，请输入内容");
            //判断是否包含规定字符,
            if (!Letter.Contains(",") && !Letter.Contains("，"))
                throw new ArgumentException("自定义标识中未包含 ' , ',参考 '是,否' || '是，否' ");
            //判断规定字符个数,
            if (Letter.Split().Where(d => d.Contains(",") || d.Contains("，")).Count() > 1)
                throw new ArgumentException("自定义标识错误 ' , ',参考 '是,否' || '是，否' ");

            //拆解数据
            var SplitLetter = Letter.Split(",").Length == 0 ? Letter.Split(",") : Letter.Split("，");
            return Value ? SplitLetter[0] : SplitLetter[1];
        }

        /// <summary>
        /// 自定义类型
        /// </summary>
        /// <param name="Value">本身</param>
        /// <param name="StartLetter">True 对应 value </param>
        /// <param name="LastLetter">False 对应 value </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string BooleanToString(this bool Value, string StartLetter, string LastLetter)
        {
            if (string.IsNullOrWhiteSpace(StartLetter) || string.IsNullOrWhiteSpace(LastLetter))
                throw new ArgumentNullException("设定值为空");

            return Value ? StartLetter : LastLetter;
        }
    }
}
