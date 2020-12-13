using System;
using System.Threading.Tasks;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ConsoleApp
{
    public class ScenarioOne : Scenario
    {
        public async Task ImportDatabase()
        {
            Init();
            await EnsureCreatedAsync();
            await ShowTasks("Before Load Database");
            await LoadDatabase();
            await ShowTasks("Before After Load Database");
            var notebook = await GetNotebook();
            await ImportData(notebook);
            await ShowTasks("After Import");
        }
    }
}