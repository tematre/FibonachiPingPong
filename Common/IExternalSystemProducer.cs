using System.Threading.Tasks;

namespace Common
{
    public interface IExternalSystemProducer<T>
    {
        Task Produce(T request);
    }
}