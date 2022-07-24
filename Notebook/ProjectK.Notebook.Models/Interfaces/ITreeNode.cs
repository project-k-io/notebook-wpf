using System.Collections.ObjectModel;

namespace ProjectK.Notebook.Models.Interfaces;

public interface ITreeNode<T>
{
    ObservableCollection<T> Nodes { get; }
}