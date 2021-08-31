using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JoreNoe.CommonInterFaces
{
    /// <summary>
    /// Api返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APIReturnInfo<T>
    {
        public static APIReturnInfo<T> Success(T Data)
        {
            return new APIReturnInfo<T> { Data = Data };
        }

        public static APIReturnInfo<T> Success(string Message)
        {
            return new APIReturnInfo<T> { Message = Message };
        }

        public static APIReturnInfo<T> Error(string Message)
        {
            return new APIReturnInfo<T> { Data = default, Message = Message, Status = false };

        }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 请求状态
        /// </summary>
        public bool Status { get; set; } = true;
    }
}
