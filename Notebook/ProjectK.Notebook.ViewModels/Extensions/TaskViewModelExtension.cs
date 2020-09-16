using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
// using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskViewModelExtension
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        public static async Task ExportToFileAsync(this NodeViewModel rootTask, string path)
        {
            var newData = new NotebookModel();
#if AK
            rootTask.SaveTo(newData.Nodes);
#endif
            await FileHelper.SaveToFileAsync(path, newData);
        }

        public static void BuildTree(this NodeViewModel rootTask, List<NodeModel> tasks)
        {
            // 
            var index = new SortedList<Guid, NodeViewModel>();

            // build index
            foreach (var task in tasks)
            {
                if (!index.ContainsKey(task.Id))
                    index.Add(task.Id, new NodeViewModel { Model = task });
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


        public static (bool ok, NodeViewModel task) FindNode(this NodeViewModel task1, Func<NodeViewModel, bool> check)
        {
            var node = task1;
            while (node != null)
            {
                if (check(node))
                    return (true, node);

                node = (NodeViewModel)node.Parent;
            }
            return (false, null);
        }

    }
}


