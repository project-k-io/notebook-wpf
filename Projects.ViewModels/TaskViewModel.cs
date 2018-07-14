// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.TaskViewModel
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Vibor.Generic.ViewModels;
using Vibor.Helpers;

namespace Projects.ViewModels
{
  public class TaskViewModel : BaseViewModel, XTask.ITask<TaskViewModel>
  {
    private ObservableCollection<TaskViewModel> _subTasks = new ObservableCollection<TaskViewModel>();
    private bool _isExpanded;
    private bool _isSelected;
    private TimeSpan _total;
    private static int _rating;

    public Projects.Models.Versions.Version2.TaskModel Model { get; set; }

    public ObservableCollection<string> TypeList { get; set; }

    public ObservableCollection<string> ContextList { get; set; }

    public ObservableCollection<string> TaskTitleList { get; set; }

    public TaskViewModel()
    {
      this.Parent = (TaskViewModel) null;
      this.Model = new Projects.Models.Versions.Version2.TaskModel();
    }

    public TaskViewModel(string title, int rating)
    {
      this.Model = new Projects.Models.Versions.Version2.TaskModel();
      this.Parent = (TaskViewModel) null;
      this.Title = title;
      this.Rating = rating;
    }

    public string Context
    {
      get
      {
        return this.Model.Context;
      }
      set
      {
        if (this.Model.Context == value)
          return;
        this.Model.Context = value;
        this.OnPropertyChanged(nameof (Context));
      }
    }

    public int Rating
    {
      get
      {
        return this.Model.Rating;
      }
      set
      {
        if (this.Model.Rating == value)
          return;
        this.Model.Rating = value;
        this.OnPropertyChanged(nameof (Rating));
      }
    }

    public TaskViewModel Parent { get; set; }

    public ObservableCollection<TaskViewModel> SubTasks
    {
      get
      {
        return this._subTasks;
      }
      set
      {
        this._subTasks = value;
      }
    }

    public Guid Id
    {
      get
      {
        return this.Model.Id;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this._isSelected;
      }
      set
      {
        if (this._isSelected == value)
          return;
        this._isSelected = value;
        this.OnPropertyChanged(nameof (IsSelected));
      }
    }

    public bool IsExpanded
    {
      get
      {
        return this._isExpanded;
      }
      set
      {
        if (this._isExpanded == value)
          return;
        this._isExpanded = value;
        this.OnPropertyChanged("IsExpnaded");
      }
    }

    public string Description
    {
      get
      {
        return this.Model.Description;
      }
      set
      {
        if (this.Model.Description == value)
          return;
        this.Model.Description = value;
        this.OnPropertyChanged(nameof (Description));
      }
    }

    public string Type
    {
      get
      {
        return this.Model.Type;
      }
      set
      {
        if (this.Model.Type == value)
          return;
        this.Model.Type = value;
        this.OnPropertyChanged(nameof (Type));
      }
    }

    public string SubType
    {
      get
      {
        return this.Model.SubType;
      }
      set
      {
        if (this.Model.SubType == value)
          return;
        this.Model.SubType = value;
        this.OnPropertyChanged(nameof (SubType));
      }
    }

    public bool IsPersonalType
    {
      get
      {
        if (string.IsNullOrEmpty(this.Type))
          return false;
        string upper = this.Type.ToUpper();
        return upper.Contains("LUNCH") || upper.Contains("PERSONAL");
      }
    }

    public bool IsSubTypeSleep
    {
      get
      {
        if (string.IsNullOrEmpty(this.SubType))
          return false;
        return this.SubType.ToUpper().Contains("SLEEP");
      }
    }

    public TimeSpan Duration
    {
      get
      {
        if (this.DateStarted == DateTime.MinValue || this.DateEnded == DateTime.MinValue)
          return TimeSpan.Zero;
        return this.DateEnded - this.DateStarted;
      }
    }

    public DateTime DateStarted
    {
      get
      {
        return this.Model.DateStarted;
      }
      set
      {
        if (this.Model.DateStarted == value)
          return;
        this.Model.DateStarted = value;
        this.OnPropertyChanged(nameof (DateStarted));
        this.OnPropertyChanged("TimeStarted");
        this.OnPropertyChanged("Duration");
      }
    }

    public DateTime DateEnded
    {
      get
      {
        return this.Model.DateEnded;
      }
      set
      {
        if (this.Model.DateEnded == value)
          return;
        this.Model.DateEnded = value;
        this.OnPropertyChanged(nameof (DateEnded));
        this.OnPropertyChanged("TimeEnded");
        this.OnPropertyChanged("Duration");
      }
    }

    public string Title
    {
      get
      {
        return this.Model.Title;
      }
      set
      {
        if (this.Model.Title == value)
          return;
        this.Model.Title = value;
        this.OnPropertyChanged(nameof (Title));
      }
    }

    public DateTime TimeStarted
    {
      get
      {
        return this.Model.DateStarted;
      }
      set
      {
        DateTime dateStarted = this.Model.DateStarted;
        DateTime dateTime = value;
        this.DateStarted = new DateTime(dateStarted.Year, dateStarted.Month, dateStarted.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        this.OnPropertyChanged(nameof (TimeStarted));
        this.OnPropertyChanged("DateStarted");
        this.OnPropertyChanged("Duration");
      }
    }

    public DateTime TimeEnded
    {
      get
      {
        return this.Model.DateEnded;
      }
      set
      {
        DateTime dateEnded = this.Model.DateEnded;
        DateTime dateTime = value;
        this.DateEnded = new DateTime(dateEnded.Year, dateEnded.Month, dateEnded.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        this.OnPropertyChanged(nameof (TimeEnded));
        this.OnPropertyChanged("DateEnded");
        this.OnPropertyChanged("Duration");
      }
    }

    public TimeSpan Total
    {
      get
      {
        return this._total;
      }
      set
      {
        if (this._total == value)
          return;
        this._total = value;
        this.OnPropertyChanged(nameof (Total));
      }
    }

    public TaskViewModel LastSubTask
    {
      get
      {
        return this.SubTasks.LastOrDefault<TaskViewModel>();
      }
    }

    public ICommand CommandSetStartedTime
    {
      get
      {
        return (ICommand) new RelayCommand(new Action(this.SetStartedTime), (Predicate<object>) null);
      }
    }

    public ICommand CommandSetEndedTime
    {
      get
      {
        return (ICommand) new RelayCommand(new Action(this.SetEndedTime), (Predicate<object>) null);
      }
    }

    public void LoadFrom(Projects.Models.Versions.Version1.TaskModel model)
    {
      this.Rating = model.Rating;
      this.IsSelected = model.IsSelected;
      this.IsExpanded = model.IsExpanded;
      this.Description = model.Description;
      this.Type = model.Type;
      this.DateStarted = model.DateStarted;
      this.DateEnded = model.DateEnded;
      this.Title = model.Title;
      if (Vibor.Generic.Models.XList.IsNullOrEmpty<Projects.Models.Versions.Version1.TaskModel>((ICollection<Projects.Models.Versions.Version1.TaskModel>) model.SubTasks))
        return;
      this.SubTasks = new ObservableCollection<TaskViewModel>();
      foreach (Projects.Models.Versions.Version1.TaskModel subTask in model.SubTasks)
      {
        TaskViewModel taskViewModel = new TaskViewModel();
        taskViewModel.LoadFrom(subTask);
        this.SubTasks.Add(taskViewModel);
      }
    }

    public void PopulateModel()
    {
      if (!(this.Model.Id == Guid.Empty))
        return;
      this.Model.Id = Guid.NewGuid();
    }

    public TaskViewModel AddNewTask()
    {
      TaskViewModel subTask = new TaskViewModel() { Title = "New Task", DateStarted = DateTime.Now, DateEnded = DateTime.Now };
      this.Add(subTask);
      int ii = this.SubTasks.IndexOf(subTask);
      this.FixContext(subTask);
      this.FixTitles(subTask, ii);
      return subTask;
    }

    public void Add(TaskViewModel subTask)
    {
      subTask.Parent = this;
      this._subTasks.Add(subTask);
    }

    public void Insert(int index, TaskViewModel subTask)
    {
      subTask.Parent = this;
      this._subTasks.Insert(index, subTask);
    }

    public void SetParents()
    {
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) this._subTasks)
      {
        subTask.Parent = this;
        subTask.SetParents();
      }
    }

    public void SetStartedTime()
    {
      this.DateStarted = DateTime.Now;
    }

    public void SetEndedTime()
    {
      this.DateEnded = DateTime.Now;
    }

    public void FixTime()
    {
      if (this.IsPersonalType)
        return;
      if (Vibor.Generic.Models.XList.IsNullOrEmpty<TaskViewModel>((ICollection<TaskViewModel>) this.SubTasks))
      {
        this.Total = this.Duration;
      }
      else
      {
        for (int index = 0; index < this.SubTasks.Count; ++index)
        {
          TaskViewModel subTask = this.SubTasks[index];
          if (subTask.DateEnded == DateTime.MinValue && index < this.SubTasks.Count - 1)
            subTask.DateEnded = this.SubTasks[index + 1].DateStarted;
        }
        this.Total = TimeSpan.Zero;
        for (int index = 0; index < this.SubTasks.Count; ++index)
        {
          TaskViewModel subTask = this.SubTasks[index];
          subTask.FixTime();
          this.Total += subTask.Total;
        }
        TaskViewModel subTask1 = this.SubTasks[this.SubTasks.Count - 1];
        if (subTask1.DateEnded != DateTime.MinValue)
          this.DateEnded = subTask1.DateEnded;
        TaskViewModel subTask2 = this.SubTasks[0];
        if (subTask2.DateStarted != DateTime.MinValue)
          this.DateStarted = subTask2.DateStarted;
      }
    }

    public void ExtractContext(ObservableCollection<string> contextList)
    {
      if (!string.IsNullOrEmpty(this.Context) && !contextList.Contains(this.Context))
        contextList.Add(this.Context);
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) this.SubTasks)
        subTask.ExtractContext(contextList);
    }

    private void FixContext(string parent, string child, TaskViewModel subTask)
    {
      if (!(this.Context == parent))
        return;
      subTask.Context = child;
    }

    private void FixContext(TaskViewModel subTask)
    {
      this.FixContext("Time Tracker", "Year", subTask);
      this.FixContext("Year", "Month", subTask);
      this.FixContext("Month", "Week", subTask);
      this.FixContext("Week", "Day", subTask);
      this.FixContext("Day", "Task", subTask);
      this.FixContext("Task", "Task", subTask);
    }

    public void FixContext()
    {
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) this.SubTasks)
      {
        this.FixContext(subTask);
        subTask.FixContext();
      }
    }

    private void FixTitles(string parent, Func<int, TaskViewModel, string> getTitle, TaskViewModel subTask, int ii)
    {
      if (!(this.Context == parent))
        return;
      subTask.Title = getTitle(ii, subTask);
    }

    public void FixTitles(TaskViewModel subTask, int ii)
    {
      Func<int, TaskViewModel, string> getTitle1 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("yyyy"));
      Func<int, TaskViewModel, string> getTitle2 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("MMMM"));
      Func<int, TaskViewModel, string> getTitle3 = (Func<int, TaskViewModel, string>) ((i, t) => "Week" + (i + 1).ToString());
      Func<int, TaskViewModel, string> getTitle4 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.DayOfWeek.ToString());
      this.FixTitles("Time Tracker", getTitle1, subTask, ii);
      this.FixTitles("Year", getTitle2, subTask, ii);
      this.FixTitles("Month", getTitle3, subTask, ii);
      this.FixTitles("Week", getTitle4, subTask, ii);
    }

    public void FixTitles()
    {
      for (int ii = 0; ii < this.SubTasks.Count; ++ii)
      {
        TaskViewModel subTask = this.SubTasks[ii];
        this.FixTitles(subTask, ii);
        subTask.FixTitles();
      }
    }

    public void FixTypes()
    {
      if (string.IsNullOrEmpty(this.Type))
      {
        string title = this.Title;
        string upper = title.ToUpper();
        if (upper.Contains("LUNCH") || upper.Contains("BREAKFAST"))
          this.Type = "Lunch";
        else if (upper.Contains("TASK") || upper.Contains("CODE REVIEW") || title.Contains("TA") || title.Contains("US"))
          this.Type = "Dev";
        else if (upper.Contains("BUILD"))
          this.Type = "Build";
        else if (upper.Contains("TIME SHEET") || upper.Contains("TIMESHEET") || upper.Contains("EMAIL") || upper.Contains("PAPER WORKS"))
          this.Type = "Misc";
        else if (upper.Contains("TALKED") || upper.Contains("MEETING") || upper.Contains("SHOWED"))
          this.Type = "Meeting";
        else if (upper.Contains("Trouble"))
          this.Type = "Support";
      }
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) this.SubTasks)
        subTask.FixTypes();
    }

    public TaskViewModel FindTask(Guid id)
    {
      if (this.Id == id)
        return this;
      foreach (TaskViewModel subTask in (Collection<TaskViewModel>) this.SubTasks)
      {
        TaskViewModel task = subTask.FindTask(id);
        if (task != null)
          return task;
      }
      return (TaskViewModel) null;
    }

    public override string ToString()
    {
      return this.Model.ToString();
    }

    public static void OnTreeViewKeyDown(TaskViewModel task, TaskViewModel.KeyStates key, Func<TaskViewModel.KeyboardStates> getState, Action handled, Action<TaskViewModel> selectItem, Action<TaskViewModel> expandItem, Func<bool> deleteMessageBox, Action<Action> dispatcher)
    {
      switch (key)
      {
        case TaskViewModel.KeyStates.Insert:
          TaskViewModel taskViewModel1;
          if (getState() == TaskViewModel.KeyboardStates.IsShiftPressed)
          {
            taskViewModel1 = task.Parent.AddNewTask();
            taskViewModel1.DateStarted = task.DateEnded;
          }
          else if (getState() == TaskViewModel.KeyboardStates.IsControlPressed)
          {
            TaskViewModel lastSubTask = task.Parent.LastSubTask;
            taskViewModel1 = task.Parent.AddNewTask();
            if (lastSubTask != null)
            {
              taskViewModel1.Type = task.Type;
              taskViewModel1.Title = task.Title;
              taskViewModel1.DateStarted = lastSubTask.DateEnded;
            }
          }
          else
            taskViewModel1 = task.AddNewTask();
          taskViewModel1.Rating = TaskViewModel._rating++;
          task.IsSelected = true;
          selectItem(task);
          expandItem(task);
          handled();
          break;
        case TaskViewModel.KeyStates.Delete:
          if (deleteMessageBox())
            break;
          TaskViewModel parent = task.Parent;
          if (parent == null)
            break;
          int num1 = parent.SubTasks.IndexOf(task);
          dispatcher((Action) (() => parent.SubTasks.Remove(task)));
          TaskViewModel taskViewModel2 = num1 > 0 ? parent.SubTasks[num1 - 1] : parent;
          if (taskViewModel2 == null)
            break;
          selectItem(taskViewModel2);
          handled();
          break;
        case TaskViewModel.KeyStates.Left:
          if (getState() == TaskViewModel.KeyboardStates.IsCtrlShiftPressed)
          {
            TaskViewModel parent1 = task.Parent;
            if (parent1 == null)
              break;
            TaskViewModel parent2 = parent1.Parent;
            if (parent2 == null)
              break;
            parent1.SubTasks.Remove(task);
            int num2 = parent2.SubTasks.IndexOf(parent1);
            parent2.Insert(num2 + 1, task);
            selectItem(task);
            handled();
            break;
          }
          break;
        case TaskViewModel.KeyStates.Right:
          if (getState() == TaskViewModel.KeyboardStates.IsCtrlShiftPressed)
          {
            TaskViewModel parent1 = task.Parent;
            if (parent1 == null)
              break;
            int num2 = parent1.SubTasks.IndexOf(task);
            if (num2 <= 0)
              break;
            TaskViewModel subTask = parent1.SubTasks[num2 - 1];
            if (subTask == null)
              break;
            parent1.SubTasks.Remove(task);
            subTask.Add(task);
            selectItem(task);
            parent1.IsExpanded = true;
            task.IsSelected = true;
            handled();
            break;
          }
          break;
        case TaskViewModel.KeyStates.Up:
          if (getState() == TaskViewModel.KeyboardStates.IsCtrlShiftPressed)
          {
            TaskViewModel parent1 = task.Parent;
            if (parent1 == null)
              break;
            int num2 = parent1.SubTasks.IndexOf(task);
            if (num2 <= 0)
              break;
            parent1.SubTasks.Remove(task);
            parent1.Insert(num2 - 1, task);
            selectItem(task);
            parent1.IsExpanded = true;
            task.IsSelected = true;
            handled();
            break;
          }
          break;
        case TaskViewModel.KeyStates.Down:
          if (getState() == TaskViewModel.KeyboardStates.IsCtrlShiftPressed)
          {
            TaskViewModel parent1 = task.Parent;
            if (parent1 == null)
              break;
            int num2 = parent1.SubTasks.IndexOf(task);
            if (num2 >= parent1.SubTasks.Count - 1)
              break;
            parent1.SubTasks.Remove(task);
            parent1.Insert(num2 + 1, task);
            selectItem(task);
            parent1.IsExpanded = true;
            task.IsSelected = true;
            handled();
          }
          break;
      }
    }

    public enum KeyboardStates
    {
      None,
      IsShiftPressed,
      IsControlPressed,
      IsCtrlShiftPressed,
    }

    public enum KeyStates
    {
      None,
      Insert,
      Delete,
      Left,
      Right,
      Up,
      Down,
    }
  }
}
