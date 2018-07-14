// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.ProjectViewModel
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vibor.Generic.ViewModels;
using Vibor.Helpers;

namespace Projects.ViewModels
{
  public class ProjectViewModel : BaseViewModel
  {
    private TaskViewModel _rootTask = new TaskViewModel();
    private readonly ObservableCollection<TaskViewModel> _selectedTaskList = new ObservableCollection<TaskViewModel>();
    private ObservableCollection<string> _contextList = new ObservableCollection<string>();
    private TaskViewModel _selectedTreeTask;
    private TaskViewModel _selectedTask;

    public ObservableCollection<TaskViewModel> SelectedTaskList
    {
      get
      {
        return this._selectedTaskList;
      }
    }

    public TaskViewModel RootTask
    {
      get
      {
        return this._rootTask;
      }
      set
      {
        this._rootTask = value;
      }
    }

    public TaskViewModel SelectedTreeTask
    {
      get
      {
        return this._selectedTreeTask;
      }
      set
      {
        if (this._selectedTreeTask == value)
          return;
        this._selectedTreeTask = value;
        this.OnPropertyChanged(nameof (SelectedTreeTask));
      }
    }

    public TaskViewModel SelectedTask
    {
      get
      {
        return this._selectedTask;
      }
      set
      {
        if (this._selectedTask == value)
          return;
        this._selectedTask = value;
        this.OnPropertyChanged(nameof (SelectedTask));
      }
    }

    public ObservableCollection<string> ContextList
    {
      get
      {
        return this._contextList;
      }
      set
      {
        this._contextList = value;
      }
    }

    public List<DateTime> GetSelectedDays()
    {
      List<DateTime> dateTimeList = new List<DateTime>();
      foreach (TaskViewModel selectedTask in (Collection<TaskViewModel>) this.SelectedTaskList)
      {
        if (!(selectedTask.Context != "Day"))
          dateTimeList.Add(selectedTask.DateStarted);
      }
      return dateTimeList;
    }

    public event EventHandler SelectedDaysChanged = null;

    public void OnSelectedDaysChanged()
    {
      EventHandler selectedDaysChanged = this.SelectedDaysChanged;
      if (selectedDaysChanged == null)
        return;
      selectedDaysChanged((object) this, EventArgs.Empty);
    }

    public TaskViewModel FindTask(Guid id)
    {
      return this.RootTask.FindTask(id);
    }

    public void SelectTreeTask(TaskViewModel task)
    {
      if (task == null)
        return;
      this.SelectedTreeTask = task;
      this.SelectedTaskList.Clear();
      XTask.AddToList<TaskViewModel>((ICollection<TaskViewModel>) this.SelectedTaskList, task);
      this.OnSelectedDaysChanged();
      this.SelectedTask = !Vibor.Generic.Models.XList.IsNullOrEmpty<TaskViewModel>((ICollection<TaskViewModel>) this.SelectedTaskList) ? this.SelectedTaskList[0] : task;
      this.OnPropertyChanged("SelectedTaskList");
    }

    public void SelectTreeTask(Guid id)
    {
      this.SelectTreeTask(this.FindTask(id));
    }

    public void SelectTask(TaskViewModel task)
    {
      if (task == null)
        return;
      this.SelectedTask = task;
      this.OnPropertyChanged("SelectedTaskList");
    }

    public void SelectTask(Guid id)
    {
      this.SelectTask(this.FindTask(id));
    }

    public static bool ContainDate(IList dates, DateTime a)
    {
      foreach (object date in (IEnumerable) dates)
      {
        if (date is DateTime)
        {
          DateTime dateTime = (DateTime) date;
          if (a.Day == dateTime.Day && a.Month == dateTime.Month && a.Year == dateTime.Year)
            return true;
        }
      }
      return false;
    }

    private static void AddToList(ICollection<TaskViewModel> list, TaskViewModel task, IList dates)
    {
      if (ProjectViewModel.ContainDate(dates, task.DateStarted))
        list.Add(task);
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) task.SubTasks)
        ProjectViewModel.AddToList(list, subTask, dates);
    }

    private static void SaveTo(Projects.Models.Versions.Version2.DataModel model, TaskViewModel task)
    {
      model.Tasks.Add(task.Model);
      if (task.Model.Id == Guid.Empty)
        task.Model.Id = Guid.NewGuid();
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) task.SubTasks)
      {
        ProjectViewModel.SaveTo(model, subTask);
        subTask.Model.ParentId = task.Model.Id;
      }
    }

    public void LoadFrom(Projects.Models.Versions.Version1.DataModel model)
    {
      this.Clear();
      this.RootTask.LoadFrom(model.RootTask);
    }

    public void LoadFrom(Projects.Models.Versions.Version2.DataModel model)
    {
      if (model == null)
        return;
      this.Clear();
      SortedList<Guid, TaskViewModel> sortedList = new SortedList<Guid, TaskViewModel>();
      foreach (Projects.Models.Versions.Version2.TaskModel task in model.Tasks)
      {
        if (task.ParentId == Guid.Empty)
        {
          this.RootTask.Model = task;
          sortedList.Add(task.Id, this.RootTask);
        }
        else if (!sortedList.ContainsKey(task.Id))
        {
          TaskViewModel taskViewModel = new TaskViewModel() { Model = task };
          sortedList.Add(task.Id, taskViewModel);
        }
      }
      foreach (Projects.Models.Versions.Version2.TaskModel task in model.Tasks)
      {
        if (!(task.ParentId == Guid.Empty))
          sortedList[task.ParentId].SubTasks.Add(sortedList[task.Id]);
      }
    }

    public void SaveTo(Projects.Models.Versions.Version2.DataModel model)
    {
      model.Tasks.Clear();
      ProjectViewModel.SaveTo(model, this.RootTask);
    }

    public void FixTime()
    {
      this.SelectedTreeTask.FixTime();
    }

    public void Clear()
    {
      this.RootTask.SubTasks.Clear();
    }

    public void ExtractContext()
    {
      this.ContextList.Clear();
      this.RootTask.ExtractContext(this.ContextList);
      this.OnPropertyChanged("ContextList");
    }

    public void FixContext()
    {
      this.SelectedTreeTask.FixContext();
    }

    public void FixTitles()
    {
      this.SelectedTreeTask.FixTitles();
    }

    public void FixTypes()
    {
      this.SelectedTreeTask.FixTypes();
    }

    public void UpdateSelectDayTaks(IList dates)
    {
      this.SelectedTaskList.Clear();
      ProjectViewModel.AddToList((ICollection<TaskViewModel>) this.SelectedTaskList, this.RootTask, dates);
    }
  }
}
