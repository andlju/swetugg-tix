namespace Swetugg.Tix.Infrastructure
{
    public interface IMessageDispatcher
    {
        void Dispatch(object msg, bool throwOnMissing = true);
    }
}