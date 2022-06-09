using System.Threading.Tasks;

namespace JoreNoe.Queue.RBMQ
{
    public interface ICustome
    {
        Task<T> ConSume<T>(CustomeContent<T> Context) where T : class;
    }
}
