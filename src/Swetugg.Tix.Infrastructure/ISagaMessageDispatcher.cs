namespace Swetugg.Tix.Infrastructure
{
    public interface ISagaMessageDispatcher
    {
        void Dispatch(object message);
    }
}