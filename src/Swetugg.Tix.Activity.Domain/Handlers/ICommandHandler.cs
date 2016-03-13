namespace Swetugg.Tix.Activity.Domain.Handlers
{
    public interface ICommandHandler<T>
    {
        void Handle(T cmd);
    }
}