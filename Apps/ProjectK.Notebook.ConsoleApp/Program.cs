using System.Threading.Tasks;

namespace ProjectK.Notebook.ConsoleApp;

internal class Program
{
    private static async Task Main()
    {
        var scenario = new Scenario();
        await scenario.ImportDatabase();
    }
}