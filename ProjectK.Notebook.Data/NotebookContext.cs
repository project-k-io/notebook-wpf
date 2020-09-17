using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.Data
{
    public class NotebookContext : DbContext
    {
        public DbSet<NotebookModel> Notebooks { get; set; }
        public DbSet<NodeModel> Nodes { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=notebooks.db");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            //optionsBuilder.UseLazyLoadingProxies();
            // optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
