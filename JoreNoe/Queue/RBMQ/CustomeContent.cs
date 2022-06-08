using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.Queue.RBMQ
{
    public class CustomeContent<T>
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public T Context { get; set; }
    }
}
