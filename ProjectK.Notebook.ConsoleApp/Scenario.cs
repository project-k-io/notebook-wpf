using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;
using TaskModel = ProjectK.Notebook.Domain.TaskModel;

namespace ProjectK.Notebook.ConsoleApp
{
    public class Scenario
    {
        #region Static Fields
        private static readonly ILogger Logger = LogManager.GetLogger<Scenario>();
        #endregion

        private const string ConnectionString = "Data Source=D:\\db\\test_notebooks2.db";
        const string DataPath = @"D:\Data\Alan.json";
        private Storage _db = new Storage();


        public async Task ImportDatabase()
        {
            _db.OpenDatabase(ConnectionString);
            await  _db.ShowTasks("Before Import");
            var notebook = await _db.GetNotebook(DataPath);
            var tasks = await ImportHelper.ReadFromFileVersionTwo(DataPath);
            await _db.ImportData(notebook, tasks);
            await _db.ShowTasks("After Import");
        }
    }
}