using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Versions.Version1;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using TaskModel = ProjectK.Notebook.Domain.TaskModel; //using ;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : NodeViewModel
    {
        private readonly ILogger _logger = LogManager.GetLogger<NotebookViewModel>();

        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;

        public NotebookViewModel()
        {
            Kind = "Notebook";
        }

        public NotebookViewModel(NotebookModel model) : this()
        {
            Model = model;
        }

        public ObservableCollection<NodeViewModel> SelectedNodeList { get; } =
            new ObservableCollection<NodeViewModel>();


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

        public string Title
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        #region Storage Functions Ver 1

        public void LoadFrom(DataModel model)
        {
            Clear();
#if AK // Load ver 1
            RootTask.LoadFrom(model.RootTask);
#endif
        }

        #endregion


        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedNode in SelectedNodeList)
                if (selectedNode.Context == "Day")
                    if (selectedNode.Model is TaskModel selectedTask)
                        dateTimeList.Add(selectedTask.DateStarted);

            return dateTimeList;
        }

        public event EventHandler SelectedDaysChanged;

        public void OnSelectedDaysChanged()
        {
            SelectedDaysChanged?.Invoke(this, EventArgs.Empty);
        }

        public NodeViewModel FindTask(Guid id)
        {
            return FindNode(id);
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
            if (node.Model is TaskModel task)
                if (ContainDate(dates, task.DateStarted))
                    list.Add(node);

            foreach (var subTask in node.Nodes)
                AddToList(list, subTask, dates);
        }


        public void FixTime()
        {
#if AK
            if (SelectedTreeNode.Model is TaskModel task)
                task.FixTime();
#endif
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public void ExtractContext()
        {
            ContextList.Clear();
            ExtractContext(ContextList);
            RaisePropertyChanged("ContextList");
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
            AddToList(SelectedNodeList, this, dates);
        }


        public async Task ExportSelectedAllAsText(string text)
        {
            var path = Title;
            var name = SelectedNode.Name;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedNode.Name, ".txt");
            if (!ok)
                return;

            await File.WriteAllTextAsync(exportPath, text);
        }

        public async Task ExportSelectedAllAsJson()
        {
            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedNode.Name);
            if (!ok)
                return;

            await SelectedNode.ExportToFileAsync(exportPath);
        }

        #region Storage Functions

        #endregion
    }
}