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
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ViewModelBase
    {
        private readonly ILogger Logger = LogManager.GetLogger<NotebookViewModel>();

        private NodeViewModel _selectedTask;
        private NodeViewModel _selectedTreeTask;
        private NotebookModel _notebook;

        public NotebookViewModel()
        {
            RootTask.Add(new NodeViewModel("Time Tracker")
            {
                Context = "Time Tracker"
            });

            _notebook = new NotebookModel();
        }

        #region Storage Functions Ver 1

        public void LoadFrom(Notebook.Domain.Versions.Version1.DataModel model)
        {
            Clear();
#if AK1
            RootTask.LoadFrom(model.RootTask);
#endif
        }


#endregion

#region Storage Functions 
        public void CopyFromViewModelToModels()
        {
            var nodes = new List<IItem>(); 
            RootTask.SaveTo(nodes);
            _notebook.Nodes.Clear();

            foreach (var node in nodes)
            {
                if (node is TaskModel task)
                {
                    _notebook.Nodes.Add(task);
                }
            }
        }



        public void PopulateFromModel(NotebookModel notebook, string name)
        {
            // created notebook task
            RootTask.Id = Guid.NewGuid();
            RootTask.Title = name;

            // load notebook 
            _notebook = notebook;

            var model = _notebook?.Copy();
            if (model == null)
                return;

            var tasks = model.Nodes;
            Clear();

            // Build Tree
            RootTask.BuildTree(tasks);
        }



#endregion

        public ObservableCollection<NodeViewModel> SelectedTaskList { get; } = new ObservableCollection<NodeViewModel>();

        public NodeViewModel RootTask { get; set; } = new NodeViewModel();

        public NodeViewModel SelectedTreeTask
        {
            get => _selectedTreeTask;
            set => Set(ref _selectedTreeTask, value);
        }

        public NodeViewModel SelectedTask
        {
            get => _selectedTask;
            set => Set(ref _selectedTask, value);
        }

        public ObservableCollection<string> ContextList { get; set; } = new ObservableCollection<string>();
        public string Title  => _notebook.Name;


        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
#if AK1
            foreach (var selectedTask in SelectedTaskList)
                if (selectedTask.Context == "Day")
                    dateTimeList.Add(selectedTask.DateStarted);
#endif
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

            SelectedTreeTask = task;
            SelectedTaskList.Clear();
#if AK1
            SelectedTaskList.AddToList(task);
#endif
            OnSelectedDaysChanged();
            SelectedTask = !SelectedTaskList.IsNullOrEmpty() ? SelectedTaskList[0] : task;
            RaisePropertyChanged("SelectedTaskList");
        }

        public void SelectTreeTask(Guid id)
        {
            SelectTreeTask(FindTask(id));
        }

        public void SelectTask(NodeViewModel task)
        {
            if (task == null)
                return;
            SelectedTask = task;
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

        private static void AddToList(ICollection<NodeViewModel> list, NodeViewModel task, IList dates)
        {
#if AK1
            if (ContainDate(dates, task.DateStarted))
                list.Add(task);
#endif
            foreach (var subTask in task.Nodes)
                AddToList(list, (NodeViewModel)subTask, dates);
        }




        public void FixTime()
        {
#if AK1
            SelectedTreeTask.FixTime();
#endif
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
            SelectedTreeTask.FixContext();
        }

        public void FixTitles()
        {
#if AK1
            SelectedTreeTask.FixTitles();
#endif
        }

        public void FixTypes()
        {
#if AK1
            SelectedTreeTask.FixTypes();
#endif
        }

        public void UpdateSelectDayTasks(IList dates)
        {
            SelectedTaskList.Clear();
            AddToList(SelectedTaskList, RootTask, dates);
        }



        public async Task ExportSelectedAllAsText(string text)
        {

            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedTask.Title, ".txt");
            if (!ok)
                return;

            await File.WriteAllTextAsync(exportPath, text);
        }

        public async Task ExportSelectedAllAsJson()
        {
            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedTask.Title);
            if (!ok)
                return;

            await SelectedTask.ExportToFileAsync(exportPath);
        }


    }
}