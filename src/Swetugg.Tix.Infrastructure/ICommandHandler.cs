namespace Swetugg.Tix.Infrastructure
{
    public interface ICommandHandler<T>
    {
        void Handle(T cmd);
    }
}