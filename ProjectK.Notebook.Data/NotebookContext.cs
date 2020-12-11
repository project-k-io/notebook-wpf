using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.Data
{
    public class NotebookContext : DbContext
    {
        private string _connectionString;
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
    }
}
