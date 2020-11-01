namespace Swetugg.Tix.Infrastructure
{
    public interface IViewEntity<TView>
    {
        TView ToView();
        void FromView(TView view);
    }
}