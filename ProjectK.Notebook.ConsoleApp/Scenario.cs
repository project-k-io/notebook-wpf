using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;
using TaskModel = ProjectK.Notebook.Domain.TaskModel;

namespace ProjectK.Notebook.ConsoleApp
{
    public class Scenario
    {
        private const string ConnectionString = "Data Source=D:\\db\\test_notebooks2.db";
        const string DataPath = @"D:\Data\Alan.json";
        private NotebookContext _db;

        //public void Init() => _db = new NotebookContext(ConnectionString);
        //public async Task EnsureCreatedAsync() => await _db.Database.EnsureCreatedAsync();
        //public async Task<NotebookModel> GetNotebook() => await _db.GetNotebook(DataPath);
        //public async Task ImportData(NotebookModel notebook) => await _db.ImportData(notebook, DataPath);
        //public async Task LoadDatabase() => await _db.Notebooks.LoadAsync();

        public async Task ShowTasks(string text)
        {
            var tasks = await _db.Tasks.ToListAsync();
            Console.WriteLine($"{text}: TaskModel count is {tasks.Count}");
            foreach (var task in tasks)
            {
                Console.WriteLine(task.ToString());
            }
        }

        public async Task ImportDatabase()
        {
            _db = new NotebookContext(ConnectionString);
            await _db.Database.EnsureCreatedAsync();
            await ShowTasks("Before Load Database");
            await _db.Database.EnsureCreatedAsync();
            await ShowTasks("Before After Load Database");
            var notebook = await _db.GetNotebook(DataPath);
            var tasks = await ImportHelper.ReadFromFileVersionTwo(DataPath);
            await _db.ImportData(notebook, tasks);
            await ShowTasks("After Import");
        }

    }
}