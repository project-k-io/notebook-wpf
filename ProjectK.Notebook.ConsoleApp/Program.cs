using System.Threading.Tasks;

namespace ProjectK.Notebook.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var scenario = new ScenarioOne();
            // await scenario.AddOneTask();
            await scenario.ImportDatabase();
         }
    }
}
