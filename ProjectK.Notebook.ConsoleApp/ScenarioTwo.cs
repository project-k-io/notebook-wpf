using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectK.Notebook.Data;

namespace ProjectK.Notebook.ConsoleApp
{
    class ScenarioTwo
    {
        private readonly NotebookContext _context;
        private ObservableCollection<Domain.Notebook> _notebooks;
        public ScenarioTwo(NotebookContext context)
        {
            this._context = context;
        }

        public void Run()
        {
            // Load
            _context.Notebooks.Load();
            _notebooks = _context.Notebooks.Local.ToObservableCollection();
            // Add Notebook
            var notebook = AddNotebook("One");
            GetTasks("Before Add" + ":");
            AddTask(notebook);
            GetTasks("After Add:");
        }

        Domain.Notebook AddNotebook(string name)
        {
            var notebook = new Domain.Notebook();
            notebook.Name = name;
            _notebooks.Add(notebook);
            _context.SaveChanges();
            return notebook;
        }

        void AddTask(Domain.Notebook notebook)
        {
            var task = new ProjectK.Notebook.Domain.Task
            {
                // Id = new Guid("00000000-0000-0000-0000-000000000001"),
                 Id = Guid.NewGuid(),
                Name = "Alan", 
                Context = "Help", 
            };
            notebook.Tasks.Add(task);
            _context.SaveChanges();
        }

        void GetTasks(string text)
        {
            Console.WriteLine($"{text}: Notebook count is {_notebooks.Count}");
            foreach (var notebook in _notebooks)
            {
                Console.WriteLine(notebook.Name);
                var tasks = notebook.Tasks;
                Console.WriteLine($"{text}: Task count is {tasks.Count}");
                foreach (var task in tasks)
                {
                    Console.WriteLine(task.Name);
                }
            }
        }
    }
}
