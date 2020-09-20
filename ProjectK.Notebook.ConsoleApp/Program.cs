using System.Threading.Tasks;


namespace ProjectK.Notebook.ConsoleApp
{
    class Program
    {
        private static ProjectK.Notebook.Data.NotebookContext context = new ProjectK.Notebook.Data.NotebookContext();

        static async Task Main(string[] args)
        {
            context.Database.EnsureCreated();
            // var scenario = new ScenarioOne(context);
            var scenario = new ScenarioTwo(context);
            await scenario.Run();
         }
    }
}
