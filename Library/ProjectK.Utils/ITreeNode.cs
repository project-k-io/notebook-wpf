using System.Collections.ObjectModel;

namespace ProjectK.Utils;

public interface ITreeNode<T>
{
    ObservableCollection<T> Nodes { get; }
}