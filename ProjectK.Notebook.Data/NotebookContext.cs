using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;

namespace ProjectK.Notebook.Data
{
    public class NotebookContext : DbContext
    {
        private readonly string _connectionString;
        public NotebookContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<NotebookModel> Notebooks { get; set; }
        public DbSet<NoteModel> Notes { get; set; }
        public DbSet<NodeModel> Nodes { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.UseLazyLoadingProxies();
            // optionsBuilder.EnableSensitiveDataLogging();
        }

        public async Task ImportData(NotebookModel notebook, string path)
        {
            // Load DataModel
            var dataModel = await FileHelper.ReadFromFileAsync<Domain.Versions.Version2.DataModel>(path);

            // Add Tasks
            var tasks = new List<TaskModel>();
            foreach (var task2 in dataModel.Tasks)
            {
                var task = new TaskModel              // ImportData
                {
                    NotebookId = notebook.Id
                };  
                task.Init(task2);
                tasks.Add(task);
            }
            await Tasks.AddRangeAsync(tasks);
            await SaveChangesAsync();
        }

        public async Task<NotebookModel> GetNotebook(string path)
        {
            // Load Database
            var notebook = Notebooks.FirstOrDefault(n => n.Name == path);
            if (notebook != null)
                return notebook;

            notebook = new NotebookModel
            {
                Id = Guid.NewGuid(),
                Name = path,
                Created = DateTime.Now,
                Context = "Notebook",
                Description = ""
            };

            Notebooks.Add(notebook);
            await SaveChangesAsync();
            return notebook;
        }
    }
}
