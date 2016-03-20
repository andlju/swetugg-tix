namespace Swetugg.Tix.Infrastructure
{
    public interface IMessageHandler<T>
    {
        void Handle(T msg);
    }
}