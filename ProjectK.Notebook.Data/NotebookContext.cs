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
        public DbSet<Domain.Notebook> Notebooks { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=c:\\db\\notebooks.db");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.UseLazyLoadingProxies();
            // optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
