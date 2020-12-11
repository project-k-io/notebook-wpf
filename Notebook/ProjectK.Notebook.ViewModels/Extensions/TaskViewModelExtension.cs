using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskViewModelExtension
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        public static void ViewModelToModel(this NotebookModel notebookModel, NodeViewModel rootTask)
        {
            Logger.LogDebug($@"Populate NotebookModel {notebookModel.Name} from TreeNode {rootTask.Name}");
            var list = new List<dynamic>();
            rootTask.SaveTo(list);
            foreach (var item in list)
            {
                if(item is TaskModel task)
                    notebookModel.Tasks.Add(task);
                else if (item is NoteModel note)
                    notebookModel.Notes.Add(note);
            }
        }

        public static async Task ExportToFileAsync(this NodeViewModel rootTask, string path)
        {
            var notebook = new NotebookModel();
            notebook.ViewModelToModel(rootTask);
            await FileHelper.SaveToFileAsync(path, notebook);
        }

        public static void BuildTree(this NodeViewModel rootTask, List<ItemModel> modes)
        {
            // 
            var index = new SortedList<Guid, NodeViewModel>();

            // build index
            foreach (var model in modes)
            {
                if (!index.ContainsKey(model.Id))
                {
                    var vm = new NodeViewModel(model);
                    index.Add(model.Id, vm);
                }
            }

            foreach (var node in modes)
            {
                if (!index.ContainsKey(node.ParentId))
                {
                    rootTask.Add(index[node.Id]);
                }
                else
                {
                    index[node.ParentId].Add(index[node.Id]);
                }
            }

        }




        public static (bool ok, NodeViewModel task) FindNode(this NodeViewModel task1, Func<NodeViewModel, bool> check)
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


