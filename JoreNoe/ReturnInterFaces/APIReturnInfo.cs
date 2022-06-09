namespace JoreNoe.CommonInterFaces
{
    /// <summary>
    /// Api返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class APIReturnInfo<T>
    {
        /// <summary>
        /// 返回成功对象
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> Success(T Data)
        {
            return new APIReturnInfo<T> { Data = Data };
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> Success(string Message)
        {
            return new APIReturnInfo<T> { Message = Message };
        }

        /// <summary>
        /// 返回错误消息
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> Error(string Message)
        {
            return new APIReturnInfo<T> { Data = default, Message = Message, Status = false, State = false };
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static APIReturnInfo<T> Error(T Data, string Message)
        {
            return new APIReturnInfo<T> { Data = Data, Message = Message, Status = false, State = false };
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
