namespace Swetugg.Tix.Process
{
    public interface IMessageDispatcher
    {
        void Dispatch(object message);
    }
}