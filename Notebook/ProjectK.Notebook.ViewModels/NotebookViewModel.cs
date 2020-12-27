using ProjectK.Notebook.Models.Interfaces;

//using ;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ItemViewModel
    {
        public NotebookViewModel()
        {
            Kind = "Notebook";
        }

        public NotebookViewModel(INode model) : this()
        {
            Model = model;
        }

        #region Storage Functions

        #endregion
    }
}