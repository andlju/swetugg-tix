namespace Swetugg.Tix.Process
{
    public interface ISagaMessageDispatcher
    {
        void Dispatch(object message);
    }
}