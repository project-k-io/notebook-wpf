using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Interfaces;
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

        public async Task ImportData(NotebookModel notebook, List<TaskModel> tasks)
        {
            // Set NotebookId
            foreach (var task in tasks)
            {
                task.NotebookId = notebook.Id;
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

        public void AddModel(INode model)
        {
            if (model is TaskModel task)
                Tasks.Add(task);
            else if (model is NoteModel note)
                Notes.Add(note);
            else if (model is NodeModel node)
                Nodes.Add(node);
        }

        public async Task AddRangeAsync(List<INode> models)
        {
            var tasks = new List<TaskModel>();
            var nodes = new List<NodeModel>();
            var notes = new List<NoteModel>();
            foreach (var model in models)
            {
                if (model is TaskModel task)
                    tasks.Add(task);
                else if (model is NoteModel note)
                    notes.Add(note);
                else if (model is NodeModel node)
                    nodes.Add(node);
            }

            if (!nodes.IsNullOrEmpty())
                await Nodes.AddRangeAsync(nodes);

            if (!tasks.IsNullOrEmpty())
                await Tasks.AddRangeAsync(tasks);

            if (!notes.IsNullOrEmpty())
                await Notes.AddRangeAsync(notes);
        }
    }
}
