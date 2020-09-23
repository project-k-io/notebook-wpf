using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskViewModelExtension
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        public static void ViewModelToModel(this NotebookModel notebookModel, NodeViewModel rootTask)
        {
            Logger.LogDebug($@"Populate NotebookModel {notebookModel.Name} from TreeNode {rootTask.Model.Name}");

            var nodes = new List<NodeModel>();
            rootTask.SaveTo(nodes);
            foreach (var node in nodes)
            {
                if (node is NoteModel note)
                    notebookModel.Notes.Add(note);

                if (node is TaskModel task)
                    notebookModel.Tasks.Add(task);
            }
        }
        public static void ModelToViewModel(this NodeViewModel rootTask, NotebookModel notebookModel)
        {
            Logger.LogDebug($@"Populate TreeNode {rootTask.Model.Name} from NotebookModel {notebookModel.Name}");

            var nodes = new List<NodeModel>();
            var notes = notebookModel.Notes.Cast<NodeModel>().ToList();
            var tasks2 = notebookModel.Tasks.ToList();

            var tasks = notebookModel.Tasks.Cast<NodeModel>();
            nodes.AddRange(notes);
            nodes.AddRange(tasks);

            // Build Tree
            rootTask.BuildTree(nodes);
        }

        public static async Task ExportToFileAsync(this NodeViewModel rootTask, string path)
        {
            var notebook = new NotebookModel();
            notebook.ViewModelToModel(rootTask);
            await FileHelper.SaveToFileAsync(path, notebook);
        }

        public static void BuildTree(this NodeViewModel rootTask, List<NodeModel> models)
        {
            // 
            var index = new SortedList<Guid, NodeViewModel>();

            // build index
            foreach (var model in models)
            {
                if (!index.ContainsKey(model.NodeId))
                {
                    var vm = new NodeViewModel(model);
                    vm.Init(model);
                    index.Add(model.NodeId, vm);
                }
            }

            foreach (var node in models)
            {
                if (!index.ContainsKey(node.ParentId))
                {
                    rootTask.Add(index[node.NodeId]);
                }
                else
                {
                    index[node.ParentId].Add(index[node.NodeId]);
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


