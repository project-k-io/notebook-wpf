using System.Collections.ObjectModel;

namespace ProjectK.Utils
{
    public interface INode<T>
    {
        ObservableCollection<T> SubTasks { get; }
    }
}