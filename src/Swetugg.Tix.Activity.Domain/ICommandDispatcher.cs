namespace Swetugg.Tix.Activity.Domain
{
    public interface ICommandDispatcher
    {
        void Dispatch(object cmd);
    }
}