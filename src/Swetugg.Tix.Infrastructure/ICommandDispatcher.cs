namespace Swetugg.Tix.Infrastructure
{
    public interface ICommandDispatcher
    {
        void Dispatch(object cmd);
    }
}