using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
// using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ViewModelBase
    {
        private readonly ILogger Logger = LogManager.GetLogger<NotebookViewModel>();

        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;
        private NotebookModel _notebook;

        public NotebookViewModel()
        {
            RootNode.Add(new NodeViewModel("Time Tracker")
            {
                Context = "Time Tracker"
            });

            _notebook = new NotebookModel();
        }

        #region Storage Functions Ver 1

        public void LoadFrom(Models.Versions.Version1.DataModel model)
        {
            Clear();
            RootNode.LoadFrom(model.RootTask);
        }

        #endregion

        #region Storage Functions 
        public void CopyFromViewModelToModels()
        {
            var tasks = new List<TaskModel>();
            RootNode.SaveTo(tasks);

            foreach (var task in tasks)
            {
                
            }
            
            _notebook.Tasks.Clear();
            _notebook.Tasks.AddRange(tasks);
        }



        public void PopulateFromModel(NotebookModel notebook, string name)
        {
            // created notebook node
            RootNode.Model.TaskId = Guid.NewGuid();
            RootNode.Model.Name = name;

            // load notebook 
            _notebook = notebook;

            var model = _notebook?.Copy();
            if (model == null)
                return;

            var tasks = model.Tasks;
            Clear();

            // Build Tree
            RootNode.BuildTree(tasks);
        }



        #endregion

        public ObservableCollection<NodeViewModel> SelectedTaskList { get; } = new ObservableCollection<NodeViewModel>();

        public NodeViewModel RootNode { get; set; } = new NodeViewModel();

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
        public string? Title  => _notebook.Name;


        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedTask in SelectedTaskList)
                if (selectedTask.Context == "Day")
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
            return RootNode.FindTask(id);
        }

        public void SelectTreeTask(NodeViewModel node)
        {
            if (node == null)
                return;

            SelectedTreeNode = node;
            SelectedTaskList.Clear();
            SelectedTaskList.AddToList(node);
            OnSelectedDaysChanged();
            SelectedNode = !SelectedTaskList.IsNullOrEmpty() ? SelectedTaskList[0] : node;
            RaisePropertyChanged("SelectedTaskList");
        }

        public void SelectTreeTask(Guid id)
        {
            SelectTreeTask(FindTask(id));
        }

        public void SelectTask(NodeViewModel node)
        {
            if (node == null)
                return;
            SelectedNode = node;
            RaisePropertyChanged("SelectedTaskList");
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
            if (ContainDate(dates, node.DateStarted))
                list.Add(node);
            foreach (var subTask in node.SubTasks)
                AddToList(list, subTask, dates);
        }




        public void FixTime()
        {
            SelectedTreeNode.FixTime();
        }

        public void Clear()
        {
            RootNode.SubTasks.Clear();
        }

        public void ExtractContext()
        {
            ContextList.Clear();
            RootNode.ExtractContext(ContextList);
            RaisePropertyChanged("ContextList");
        }

        public void FixContext()
        {
            SelectedTreeNode.FixContext();
        }

        public void FixTitles()
        {
            SelectedTreeNode.FixTitles();
        }

        public void FixTypes()
        {
            SelectedTreeNode.FixTypes();
        }

        public void UpdateSelectDayTasks(IList dates)
        {
            SelectedTaskList.Clear();
            AddToList(SelectedTaskList, RootNode, dates);
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