using System.Threading.Tasks;
using ProjectK.Notebook.Data;

namespace ProjectK.Notebook.ConsoleApp
{
    public class Scenario
    {
        private const string ConnectionString = "Data Source=D:\\db\\test_notebooks2.db";
        private const string DataPath = @"D:\Data\Alan.json";
        private readonly Storage _db = new Storage();

        public async Task ImportDatabase()
        {
            _db.OpenDatabase(ConnectionString);
            await _db.ShowTasks("Before Import");
            var notebook = await _db.GetNotebook(DataPath);
            var tasks = await ImportHelper.ReadFromFileVersionTwo(DataPath);
            await _db.ImportData(notebook, tasks);
            await _db.ShowTasks("After Import");
        }
    }
}