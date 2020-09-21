using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using Task = System.Threading.Tasks.Task;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ViewModelBase
    {
        private readonly ILogger _logger = LogManager.GetLogger<NotebookViewModel>();

        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;
        private NotebookModel _notebookModel;

        public NotebookViewModel()
        {
            RootTask.Add(new NodeViewModel("Time Tracker")
            {
                Context = "Time Tracker"
            });

            _notebookModel = new NotebookModel();
        }

        public NotebookViewModel(NotebookModel notebookModel)
        {
            _notebookModel = notebookModel;
        }


        #region Storage Functions Ver 1

        public void LoadFrom(Domain.Versions.Version1.DataModel model)
        {
            Clear();
#if AK  // Load ver 1
            RootTask.LoadFrom(model.RootTask);
#endif
        }


        #endregion

        #region Storage Functions 

        public void ViewModelToModel()
        {
            _logger.LogDebug($"Populate NotebookModel from TreeNode {RootTask.Title}");
            _notebookModel.Clear();
            _notebookModel.ViewModelToModel(RootTask);
        }

        public void ModelToViewModel()
        {
            _logger.LogDebug($"Populate TreeNode from NotebookModel {_notebookModel.Name}");
            // created notebookModel node
            RootTask.Id = Guid.NewGuid();
            RootTask.Title = _notebookModel.Name;

            // load notebookModel 
            RootTask.ModelToViewModel(_notebookModel);
        }



        #endregion

        public ObservableCollection<NodeViewModel> SelectedNodeList { get; } = new ObservableCollection<NodeViewModel>();

        public NodeViewModel RootTask { get; set; } = new NodeViewModel();

        public NodeViewModel SelectedTreeNode
        {
            get => _selectedTreeNode;
            set => Set(ref _selectedTreeNode, value);
        }

        public NodeViewModel SelectedNode
        {
            get => _selectedNode;
            set => Set(ref _selectedNode, value);
        }

        public ObservableCollection<string> ContextList { get; set; } = new ObservableCollection<string>();
        public string Title
        {
            get => _notebookModel.Name;
            set => _notebookModel.Name = value;
        }


        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedNode in SelectedNodeList)
            {
                if (selectedNode.Context == "Day")
                {
                    if (selectedNode is TaskViewModel selectedTask)
                        dateTimeList.Add(selectedTask.DateStarted);
                }
            }

            return dateTimeList;
        }

        public event EventHandler SelectedDaysChanged;

        public void OnSelectedDaysChanged()
        {
            SelectedDaysChanged?.Invoke(this, EventArgs.Empty);
        }

        public NodeViewModel FindTask(Guid id)
        {
            return (NodeViewModel)RootTask.FindNode(id);
        }

        public void SelectTreeTask(NodeViewModel task)
        {
            if (task == null)
                return;

            SelectedTreeNode = task;
            SelectedNodeList.Clear();
            SelectedNodeList.AddToList(task);
            OnSelectedDaysChanged();
            SelectedNode = !SelectedNodeList.IsNullOrEmpty() ? SelectedNodeList[0] : task;
            RaisePropertyChanged("SelectedNodeList");
        }

        public void SelectTreeTask(Guid id)
        {
            SelectTreeTask(FindTask(id));
        }

        public void SelectTask(NodeViewModel task)
        {
            if (task == null)
                return;
            SelectedNode = task;
            RaisePropertyChanged("SelectedNodeList");
        }

        public void SelectTask(Guid id)
        {
            SelectTask(FindTask(id));
        }

        public static bool ContainDate(IList dates, DateTime a)
        {
            foreach (var date in dates)
                if (date is DateTime dateTime)
                    if (a.Day == dateTime.Day && a.Month == dateTime.Month && a.Year == dateTime.Year)
                        return true;

            return false;
        }

        private static void AddToList(ICollection<NodeViewModel> list, NodeViewModel node, IList dates)
        {
            if (node is TaskViewModel task)
            {
                if (ContainDate(dates, task.DateStarted))
                    list.Add(node);
            }

            foreach (var subTask in node.Nodes)
                AddToList(list, (NodeViewModel)subTask, dates);
        }




        public void FixTime()
        {
            if(SelectedTreeNode is TaskViewModel task)
                task.FixTime();
        }

        public void Clear()
        {
            RootTask.Nodes.Clear();
        }

        public void ExtractContext()
        {
            ContextList.Clear();
            RootTask.ExtractContext(ContextList);
            RaisePropertyChanged("ContextList");
        }

        public void FixContext()
        {
            SelectedTreeNode.FixContext();
        }

        public void FixTitles()
        {
#if AK1
            SelectedTreeNode.FixTitles();
#endif
        }

        public void FixTypes()
        {
#if AK1
            SelectedTreeNode.FixTypes();
#endif
        }

        public void UpdateSelectDayTasks(IList dates)
        {
            SelectedNodeList.Clear();
            AddToList(SelectedNodeList, RootTask, dates);
        }



        public async Task ExportSelectedAllAsText(string text)
        {

            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedNode.Title, ".txt");
            if (!ok)
                return;

            await File.WriteAllTextAsync(exportPath, text);
        }

        public async Task ExportSelectedAllAsJson()
        {
            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedNode.Title);
            if (!ok)
                return;

            await SelectedNode.ExportToFileAsync(exportPath);
        }


    }
}