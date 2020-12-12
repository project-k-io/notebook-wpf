using System;
using System.Threading.Tasks;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ConsoleApp
{
    public class ScenarioOne : Scenario
    {
        public async Task AddOneTask()
        {
            Init();
            await EnsureCreatedAsync();
            await ShowTasks("Before Add");
            await LoadDatabase();
            await ShowTasks("Before Add" + ":");
            var notebook = await GetNotebook();
            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                NotebookId = notebook.Id,
                Name = "Test",
                Description = "Created for Testing"
            };
            await AddTask(task);
            await ShowTasks("After Add:");
        }

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