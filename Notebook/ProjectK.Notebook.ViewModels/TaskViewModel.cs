using ProjectK.Notebook.Domain;
// using ProjectK.Notebook.Models.Versions.Version2;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskViewModel : NodeViewModel<TaskModel>
    {
        public TaskViewModel()
        {
        }

        public TaskViewModel(string title) : base(title)
        {
        }

    }

}