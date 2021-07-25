using Microsoft.Extensions.Options;
using ProjectK.Notebook.ViewModels.Design;
using ProjectK.Notebook.WinApp.Models;

namespace ProjectK.Notebook.WinApp.ViewModels
{
    public class DesignAppViewModel : AppViewModel
    {
        public DesignAppViewModel() : base(new OptionsManager<AppSettings>(null))
        {
            var notebook = Sample.CreateNotebook();
            RootNode.Add(notebook);
        }
    }
}