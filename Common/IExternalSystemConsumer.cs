namespace Common
{
    public interface IExternalSystemConsumer<T>
    {
        void Consume(T model);
        void RegisterHandler(IConsumerHandler<T> handler);
    }
}