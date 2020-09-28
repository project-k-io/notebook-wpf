using System.Threading.Tasks;


namespace ProjectK.Notebook.ConsoleApp
{
    class Program
    {

        static async Task Main(string[] args)
        {
            // var scenario = new ScenarioOne(context);
            var scenario = new ScenarioTwo();
            await scenario.Run();
         }
    }
}
