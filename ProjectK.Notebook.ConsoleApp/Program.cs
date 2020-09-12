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
            GetTasks("Before Add:");
            AddTask();
            GetTasks("After Add:");
         }

        private static void AddTask()
        {
            var task = new TaskModel { Name = "Alan" };
            context.Tasks.Add(task);
            context.SaveChanges();
        }

        private static void GetTasks(string text)
        {
            var tasks = context.Tasks.ToList();
            Console.WriteLine($"{text}: Task count is {tasks.Count}");
            foreach (var task in tasks)
            {
                Console.WriteLine(task.Name);
            }
        }
    }
}
