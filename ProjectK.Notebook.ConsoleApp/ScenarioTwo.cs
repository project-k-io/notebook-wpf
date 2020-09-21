using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;

namespace ProjectK.Notebook.ConsoleApp
{
    class ScenarioTwo
    {
        private readonly NotebookContext _context = new NotebookContext();
        private ObservableCollection<NotebookModel> _notebooks;
        public async Task Run()
        {
            // await CreateDatabaseAsync();
            await ShowDatabaseAsync();
        }

        public async Task CreateDatabaseAsync()
        {
            const string path = @"C:\Data\Alan.json";

            // Check
            await _context.Database.EnsureCreatedAsync();

            // Load Database
            await _context.Notebooks.LoadAsync();
            _notebooks = _context.Notebooks.Local.ToObservableCollection();

            // Show Tasks
            GetTasks("Before Add" + ":");

            // Create Notebook
            var notebook = new NotebookModel
            {
                NodeId = Guid.NewGuid(),
                Name = path,
                Created = DateTime.Now,
                Context = "Notebook",
            };

            _notebooks.Add(notebook);
            await _context.SaveChangesAsync();

            // Load DataModel
            var dataModel = await FileHelper.ReadFromFileAsync<Domain.Versions.Version2.DataModel>(path);

            // Add Tasks
            foreach (var task2 in dataModel.Tasks)
            {
                var task = new TaskModel();
                task.Init(task2);
                notebook.Tasks.Add(task);
            }
            await _context.SaveChangesAsync();

            // Show Tasks
            GetTasks("After Add:");
        }
        public async Task ShowDatabaseAsync()
        {
            // Load Database
            await _context.Notebooks.LoadAsync();
            _notebooks = _context.Notebooks.Local.ToObservableCollection();

            // Show Tasks
            GetTasks("Database" + ":");
        }



        void AddTask(NotebookModel notebookModel)
        {
            var task = new TaskModel
            {
                // NodeId = new Guid("00000000-0000-0000-0000-000000000001"),
                 NodeId = Guid.NewGuid(),
                Name = "Alan", 
                Context = "Help", 
            };
            notebookModel.Tasks.Add(task);
            _context.SaveChanges();
        }


        void GetTasks(string text)
        {
            Console.WriteLine($"{text}: NotebookModel count is {_notebooks.Count}");
            foreach (var notebook in _notebooks)
            {
                Console.WriteLine(notebook.Name);
                var tasks = notebook.Tasks;
                Console.WriteLine($"{text}: TaskModel count is {tasks.Count}");
                foreach (var task in tasks)
                {
                    Console.WriteLine(task.Name);
                }
            }
        }
    }
}
