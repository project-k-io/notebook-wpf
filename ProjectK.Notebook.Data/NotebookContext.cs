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
    }
}
