using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.Data
{
    public class Storage
    {
        #region Static Fields

        private static readonly ILogger Logger = LogManager.GetLogger<Storage>();

        #endregion

        private NotebookContext _db;


        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public List<NotebookModel> GetNonRootNotebooks()
        {
            var notebooks = _db.Notebooks.Where(n => n.NonRoot).ToList();
            return notebooks;
        }

        public List<NotebookModel> GetNotebooks()
        {
            var notebooks = _db.Notebooks.Local.ToList();
            return notebooks;
        }

        public async Task AddNotebook(NotebookModel notebook)
        {
            await _db.Notebooks.AddAsync(notebook);
        }

        public void OpenDatabase(string connectionString)
        {
            _db = new NotebookContext(connectionString);

            // this is for demo purposes only, to make it easier
            // to get up and running
            _db.Database.EnsureCreated();

            // load the entities into EF Core
            _db.Notebooks.Load();
        }

        public async Task CloseConnection()
        {
            await _db.Database.CloseConnectionAsync();
        }

        public async Task ImportData(NotebookModel notebook, List<TaskModel> tasks)
        {
            // Set NotebookId
            foreach (var task in tasks) task.NotebookId = notebook.Id;

            await _db.Tasks.AddRangeAsync(tasks);
            await SaveChangesAsync();
        }

        public async Task<NotebookModel> GetNotebook(string path)
        {
            // Load Database
            var notebook = _db.Notebooks.FirstOrDefault(n => n.Name == path);
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

            _db.Notebooks.Add(notebook);
            await SaveChangesAsync();
            return notebook;
        }

        public void AddModel(INode model)
        {
            if (model is TaskModel task)
                _db.Tasks.Add(task);
            else if (model is NoteModel note)
                _db.Notes.Add(note);
            else if (model is NodeModel node)
                _db.Nodes.Add(node);
        }

        public async Task AddRange(List<INode> models)
        {
            var tasks = new List<TaskModel>();
            var nodes = new List<NodeModel>();
            var notes = new List<NoteModel>();
            foreach (var model in models)
                if (model is TaskModel task)
                    tasks.Add(task);
                else if (model is NoteModel note)
                    notes.Add(note);
                else if (model is NodeModel node)
                    nodes.Add(node);

            if (!nodes.IsNullOrEmpty())
                await _db.Nodes.AddRangeAsync(nodes);

            if (!tasks.IsNullOrEmpty())
                await _db.Tasks.AddRangeAsync(tasks);

            if (!notes.IsNullOrEmpty())
                await _db.Notes.AddRangeAsync(notes);
        }

        public async Task<EntityEntry<NotebookModel>> Add(NotebookModel notebookModel)
        {
            return await _db.Notebooks.AddAsync(notebookModel);
        }

        public void Remove(NotebookModel notebook)
        {
            _db.Notebooks.Remove(notebook);
        }

        public void RemoveRange(List<INode> models)
        {
            _db.RemoveRange(models);
        }

        public async Task<List<TaskModel>> GetTasks()
        {
            return await _db.Tasks.ToListAsync();
        }

        public async Task ShowTasks(string text)
        {
            var tasks = await GetTasks();
            Logger.LogDebug($"{text}: TaskModel count is {tasks.Count}");
            foreach (var task in tasks) Logger.LogDebug(task.ToString());
        }
    }
}