using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Queue.RBMQ
{
    public interface ICustome<T> where T : class
    {
        Task<T> ConSume(CustomeContent<T> Context);
    }
}
