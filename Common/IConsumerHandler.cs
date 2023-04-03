namespace Common
{
    public interface IConsumerHandler<T>
    {
        void Handle(T model);
    }
}