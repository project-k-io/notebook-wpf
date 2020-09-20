using System;
using System.Linq;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ConsoleApp
{
    class Program
    {
        private static NotebookContext context = new NotebookContext();

        static void Main(string[] args)
        {
            context.Database.EnsureCreated();
            // var scenario = new ScenarioOne(context);
            var scenario = new ScenarioTwo(context);
            scenario.Run();
         }
    }
}
