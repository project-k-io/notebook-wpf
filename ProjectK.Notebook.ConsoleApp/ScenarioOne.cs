using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectK.Notebook.Data;

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
            var task = new Domain.TaskModel { Name = "Alan" };
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
