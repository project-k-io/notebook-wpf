using System.Collections.ObjectModel;

namespace ProjectK.Utils
{
    public interface ITask<T>
    {
        ObservableCollection<T> SubTasks { get; }
    }
}