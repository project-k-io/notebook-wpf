using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;

namespace ProjectK.Notebook.Data
{
    public class NotebookContext : DbContext
    {
        private static readonly ILogger Logger = LogManager.GetLogger<NotebookContext>();

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
            optionsBuilder
                .UseSqlite(_connectionString)
                .UseLazyLoadingProxies()
                // .EnableSensitiveDataLogging()
                .LogTo(s => Logger.LogDebug(s), LogLevel.Information);
        }
    }
}