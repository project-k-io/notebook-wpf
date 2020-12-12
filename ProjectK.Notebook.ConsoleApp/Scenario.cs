using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Versions.Version1;
using ProjectK.Utils;
using TaskModel = ProjectK.Notebook.Domain.TaskModel;

namespace ProjectK.Notebook.ConsoleApp
{
    public class Scenario
    {
        private const string ConnectionString = "Data Source=D:\\db\\test_notebooks2.db";
        const string DataPath = @"D:\Data\Alan.json";
        private NotebookContext _db;
        private ObservableCollection<NotebookModel> _notebooks;

        public async Task<EntityEntry<TaskModel>> AddTask(TaskModel task)
        {
            var t = await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return t;
        }


        public void Init()
        {
            _db = new NotebookContext(ConnectionString);
        }

        public async Task EnsureCreatedAsync()
        {
            await _db.Database.EnsureCreatedAsync();
        }

        public async Task LoadDatabase()
        {
            // Load Database
            await _db.Notebooks.LoadAsync();
            _notebooks = _db.Notebooks.Local.ToObservableCollection();
        }

        public async Task<NotebookModel> GetNotebook()
        {
            // Load Database
            var notebook = _notebooks.FirstOrDefault(n => n.Name == DataPath);
            if (notebook != null)
                return notebook;

            notebook = new NotebookModel
            {
                Id = Guid.NewGuid(),
                Name = DataPath,
                Created = DateTime.Now,
                Context = "Notebook",
                Description = "Create from Console App"
            };
            _notebooks.Add(notebook);
            await _db.SaveChangesAsync();

            return notebook;
        }

        public async Task ImportData(NotebookModel notebook)
        {
            // Load DataModel
            var dataModel = await FileHelper.ReadFromFileAsync<ProjectK.Notebook.Domain.Versions.Version2.DataModel>(DataPath);

            // Add Tasks
            var tasks = new List<TaskModel>();
            foreach (var task2 in dataModel.Tasks)
            {
                var task = new TaskModel { NotebookId = notebook.Id };
                task.Init(task2);
                tasks.Add(task);
            }

            await _db.Tasks.AddRangeAsync(tasks);
            await _db.SaveChangesAsync();
        }



        public async Task ShowTasks(string text)
        {
            var tasks = await _db.Tasks.ToListAsync();
            Console.WriteLine($"{text}: TaskModel count is {tasks.Count}");
            foreach (var task in tasks)
            {
                Console.WriteLine(task.ToString());
            }
        }

        void GetTasksAll(string text)
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