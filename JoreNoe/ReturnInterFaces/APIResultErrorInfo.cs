namespace JoreNoe.ReturnInterFaces
{
    public class APIResultErrorInfo<T>
    {
        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="status">状态码</param>
        /// <returns></returns>
        public static APIResultErrorInfo<T> Error(T message, int status = 200)
        {
            return new APIResultErrorInfo<T>
            {
                Data = default,
                Message = message,
                Status = status,
                State = false
            };
        }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = false;

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 请求状态
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// 消息
        /// </summary>
        public T Message { get; set; }
    }
}
