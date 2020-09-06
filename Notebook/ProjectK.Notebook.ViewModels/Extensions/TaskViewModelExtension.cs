using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskViewModelExtension
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        public static async Task ExportToFileAsync(this TaskViewModel rootTask, string path)
        {
            var newData = new DataModel();
            rootTask.SaveTo(newData.Tasks);
            await FileHelper.SaveToFileAsync(path, newData);
        }

        public static void BuildTree(this TaskViewModel rootTask, List<TaskModel> tasks)
        {
            // 
            var index = new SortedList<Guid, TaskViewModel>();

            // build index
            foreach (var task in tasks)
            {
                if (!index.ContainsKey(task.Id))
                    index.Add(task.Id, new TaskViewModel { Model = task });
            }

            foreach (var task in tasks)
            {
                if (!index.ContainsKey(task.ParentId))
                {
                    rootTask.Add(index[task.Id]);
                }
                else
                {
                    index[task.ParentId].Add(index[task.Id]);
                }
            }

        }


        public static (string fileName, bool ok) SetFileDialog(this FileDialog dialog, string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName)) dialog.InitialDirectory = directoryName;

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName)) dialog.FileName = fileName;

            dialog.DefaultExt = ".json";
            dialog.Filter = "Json documents (.json)|*.json" +
                            "|XML documents(.xml) | *.xml";

            var result = dialog.ShowDialog();
            if (result != true)
                return ("", false);

            return (dialog.FileName, true);
        }

        public static async Task ImportToSelectedAsJson(this TaskViewModel rootTask)
        {
            Logger.LogDebug("UserAction_ImportToSelectedAsJson()");

            // Select Import File
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog("");
            if (!r.ok)
                return;

            // Read Import File
            var data = await FileHelper.ReadFromFileAsync<DataModel>(r.fileName);

            // Get Tasks
            var tasks = data.Tasks;

            // Build Tree
            rootTask.BuildTree(tasks);
        }

        public static (bool ok, TaskViewModel task) FindNode(this TaskViewModel task1, Func<TaskViewModel, bool> check)
        {
            var node = task1;
            while (node != null)
            {
                if (check(node))
                    return (true, node);

                node = node.Parent;
            }
            return (false, null);
        }

    }
}
