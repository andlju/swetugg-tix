namespace Swetugg.Tix.Activity.Views
{
    public abstract class ViewItemBase
    {
        private string _typeName;

        public ViewItemBase()
        {
            _typeName = this.GetType().Name;
        }

        public string id => $"{_typeName}_{Id}";

        public abstract string Id { get; }
    }
}