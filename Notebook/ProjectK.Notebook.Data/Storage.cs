using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ProjectK.Extensions.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.ToolKit.Utils;

namespace ProjectK.Notebook.Data;

public class Storage
{
    #region Static Fields

    private static readonly ILogger Logger = LogManager.GetLogger<Storage>();

    #endregion

    private Database _database;


    public async Task SaveChangesAsync()
    {
        await _database.SaveChangesAsync();
    }

    public List<NotebookModel> GetNonRootNotebooks()
    {
        var notebooks = _database.Notebooks.Where(n => n.NonRoot).ToList();
        return notebooks;
    }

    public List<NotebookModel> GetNotebooks()
    {
        var notebooks = _database.Notebooks.Local.ToList();
        return notebooks;
    }

    public async Task AddNotebook(NotebookModel notebook)
    {
        await _database.Notebooks.AddAsync(notebook);
    }

    public void OpenDatabase(string connectionString)
    {
        try
        {
            _database = new Database(connectionString);

            // this is for demo purposes only, to make it easier
            // to get up and running
            _database.Database.EnsureCreated();

            // load the entities into EF Core
            _database.Notebooks.Load();
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    public async Task CloseConnection()
    {
        await _database.Database.CloseConnectionAsync();
    }

    public async Task ImportData(NotebookModel notebook, List<TaskModel> tasks)
    {
        // Set NotebookId
        foreach (var task in tasks) task.NotebookId = notebook.Id;

        await _database.Tasks.AddRangeAsync(tasks);
        await SaveChangesAsync();
    }

    public async Task<NotebookModel> GetNotebook(string path)
    {
        // Load Database
        var notebook = _database.Notebooks.FirstOrDefault(n => n.Name == path);
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

        _database.Notebooks.Add(notebook);
        await SaveChangesAsync();
        return notebook;
    }

    public void AddModel(INode model)
    {
        if (model is TaskModel task)
            _database.Tasks.Add(task);
        else if (model is NoteModel note)
            _database.Notes.Add(note);
        else if (model is NodeModel node)
            _database.Nodes.Add(node);
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
            await _database.Nodes.AddRangeAsync(nodes);

        if (!tasks.IsNullOrEmpty())
            await _database.Tasks.AddRangeAsync(tasks);

        if (!notes.IsNullOrEmpty())
            await _database.Notes.AddRangeAsync(notes);
    }

    public async Task<EntityEntry<NotebookModel>> Add(NotebookModel notebookModel)
    {
        return await _database.Notebooks.AddAsync(notebookModel);
    }

    public void Remove(NotebookModel notebook)
    {
        _database.Notebooks.Remove(notebook);
    }

    public void RemoveRange(List<INode> models)
    {
        _database.RemoveRange(models);
    }

    public async Task<List<TaskModel>> GetTasks()
    {
        return await _database.Tasks.ToListAsync();
    }

    public async Task ShowTasks(string text)
    {
        var tasks = await GetTasks();
        Logger.LogDebug($"{text}: TaskModel count is {tasks.Count}");
        foreach (var task in tasks) Logger.LogDebug(task.ToString());
    }

    public void DeleteNotebook(Guid id)
    {
        var notebook = _database.Notebooks.First(n => n.Id == id);
        if (notebook == null)
            return;

        _database.Notebooks.Remove(notebook);
    }

    public async Task<(bool foound, NotebookModel notebook)> FirstOrDefaultNotebook()
    {
        var notebooks = await _database.Notebooks.ToListAsync();

        // Selected Notebook
        if (notebooks.Count == 0)
            return (false, null);

        var notebook = notebooks.FirstOrDefault();
        if (notebook == null)
            return (false, null);


        return (true, notebook);
    }
}