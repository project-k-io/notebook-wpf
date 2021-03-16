using Microsoft.Extensions.Options;
using ProjectK.Notebook.ViewModels.Design;
using ProjectK.Notebook.WinApp.Settings;

namespace ProjectK.Notebook.WinApp
{
    public class DesignAppViewModel : AppViewModel
    {
        public DesignAppViewModel() : base(new OptionsManager<AppSettings>(null))
        {
            var notebook = DesignMainViewModel.CreateNotebook();
            RootNode.Add(notebook);
        }
    }
}