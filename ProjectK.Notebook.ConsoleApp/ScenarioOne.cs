using System;
using System.Linq;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ConsoleApp
{

    class ScenarioOne
    {
        private readonly NotebookContext _context;
        public ScenarioOne(NotebookContext context)
        {
            _context = context;
        }

        public void Run()
        {
            GetTasks("Before Add:");
            AddTask();
            GetTasks("After Add:");
        }

        void AddTask()
        {
            var task = new TaskModel { Name = "Alan" };
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        void GetTasks(string text)
        {
            var tasks = _context.Tasks.ToList();
            Console.WriteLine($"{text}: TaskModel count is {tasks.Count}");
            foreach (var task in tasks)
            {
                Console.WriteLine(task.Name);
            }
        }
    }
}
