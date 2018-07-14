// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.MainViewModel
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

using Generic.Models;
using log4net;
using Projects.Models;
using Projects.Models.Versions.Version2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vibor.Generic.ViewModels;
using Vibor.Helpers;

namespace Projects.ViewModels
{
  public class MainViewModel : BaseViewModel
  {
    private static readonly ILog Logger = LogManager.GetLogger(nameof (MainViewModel));
    private readonly ProjectViewModel _project = new ProjectViewModel();
    private readonly LayoutViewModel _layout = new LayoutViewModel();
    private readonly ObservableCollection<FileInfo> _mostRecentFiles = new ObservableCollection<FileInfo>();
    private string _folder;
    private string _recentFile;
    private string _report;
    private string _excelCsvText;
    private bool _useTimeOptimization;

    public ConfigModel Config { get; set; }

    public Guid LastListTaskId { get; set; }

    public Guid LastTreeTaskId { get; set; }

    public LayoutViewModel Layout
    {
      get
      {
        return this._layout;
      }
    }

    public ProjectViewModel Project
    {
      get
      {
        return this._project;
      }
    }

    public TaskViewModel RootTask
    {
      get
      {
        return this.Project.RootTask;
      }
    }

    private Assembly Assembly { get; set; }

    public string Title
    {
      get
      {
        return XAttribute.GetAssemblyTitle(this.Assembly) + " " + XAttribute.GetAssemblyVersion(this.Assembly) + " - " + this.Folder;
      }
    }

    public string Folder
    {
      get
      {
        return this._folder;
      }
      set
      {
        if (this._folder == value)
          return;
        this._folder = value;
        this.OnPropertyChanged(nameof (Folder));
        this.OnPropertyChanged("Title");
      }
    }

    public string RecentFile
    {
      get
      {
        return this._recentFile;
      }
      set
      {
        if (this._recentFile == value)
          return;
        this._recentFile = value;
        this.OnPropertyChanged(nameof (RecentFile));
        this.OnPropertyChanged("Title");
      }
    }

    public Projects.Models.Versions.Version2.DataModel Data { get; set; }

    public string Report
    {
      get
      {
        return this._report;
      }
      set
      {
        if (this._report == value)
          return;
        this._report = value;
        this.OnPropertyChanged(nameof (Report));
      }
    }

    public string ExcelCsvText
    {
      get
      {
        return this._excelCsvText;
      }
      set
      {
        if (this._excelCsvText == value)
          return;
        this._excelCsvText = value;
        this.OnPropertyChanged(nameof (ExcelCsvText));
      }
    }

    public bool UseTimeOptimization
    {
      get
      {
        return this._useTimeOptimization;
      }
      set
      {
        if (this._useTimeOptimization == value)
          return;
        this._useTimeOptimization = value;
        this.OnPropertyChanged(nameof (UseTimeOptimization));
      }
    }

    private string ConfigPath
    {
      get
      {
        string assemblyCompany = XAttribute.GetAssemblyCompany(this.Assembly);
        string assemblyProduct = XAttribute.GetAssemblyProduct(this.Assembly);
        string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), assemblyCompany);
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        string path = Path.Combine(str, assemblyProduct);
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        return path;
      }
    }

    private string ConfigFile
    {
      get
      {
        return Path.Combine(this.ConfigPath, "Config.xml");
      }
    }

    public ObservableCollection<FileInfo> MostRecentFiles
    {
      get
      {
        return this._mostRecentFiles;
      }
    }

    public ObservableCollection<string> TypeList { get; set; }

    public ObservableCollection<string> ContextList { get; set; }

    public ObservableCollection<string> TaskTitleList { get; set; }

    public void FileOpenOldFormat()
    {
      this.Project.LoadFrom(Projects.Models.Versions.Version1.DataModel.ReadFromFile(this.RecentFile));
    }

    public async Task FileSaveOldFormatAsync()
    {
      await XFile.SaveToFileAsync<ProjectViewModel>(this.Project, this.RecentFile);
    }

    public async Task FileOpenNewFormatAsync()
    {
      if (!Directory.Exists(this.Folder))
        return;
      string fileName = Projects.Models.Versions.Version2.DataModel.GetTasksFileName(this.Folder);
      this.Data = await XFile.ReadFromFileAsync<Projects.Models.Versions.Version2.DataModel>(fileName);
      this.Project.LoadFrom(this.Data);
      this.UseSettings();
    }

    public async Task FileSaveNewFormatAsync()
    {
      string fileName = Projects.Models.Versions.Version2.DataModel.GetTasksFileName(this.Folder);
      if (this.Data == null)
        this.Data = new Projects.Models.Versions.Version2.DataModel();
      XFile.SaveOldFile(fileName);
      this.Project.SaveTo(this.Data);
      await XFile.SaveToFileAsync<Projects.Models.Versions.Version2.DataModel>(this.Data, fileName);
    }

    public MainViewModel()
    {
      this.Assembly = Assembly.GetExecutingAssembly();
      this.CanSave = true;
      this.TypeList = new ObservableCollection<string>();
      this.ContextList = new ObservableCollection<string>();
      this.TaskTitleList = new ObservableCollection<string>();
    }

    public event EventHandler<EventArgs> GenerateReportChanged = null;

    public event EventHandler<TaskEventArgs> SelectedTaskChanged = null;

    public void OnSelectedTaskChanged(TaskViewModel task)
    {
      EventHandler<TaskEventArgs> selectedTaskChanged = this.SelectedTaskChanged;
      if (selectedTaskChanged == null)
        return;
      selectedTaskChanged((object) this, new TaskEventArgs()
      {
        Task = task
      });
    }

    public void OnGenerateReportChanged()
    {
      EventHandler<EventArgs> generateReportChanged = this.GenerateReportChanged;
      if (generateReportChanged == null)
        return;
      generateReportChanged((object) this, EventArgs.Empty);
    }

    public bool CanSave { get; set; }

    public async Task SaveDataAsync()
    {
      if (!this.CanSave)
        return;
      await this.SaveConfigurationAsync();
      await this.FileSaveNewFormatAsync();
    }

    public async Task LoadDataAsync()
    {
      await this.LoadConfigurationAsync();
      await this.FileOpenNewFormatAsync();
    }

    public async Task UpdateTypeListAsync()
    {
      await Task.Run((Action) (() =>
      {
        List<TaskViewModel> taskViewModelList = new List<TaskViewModel>();
        XTask.AddToList<TaskViewModel>((ICollection<TaskViewModel>) taskViewModelList, this.RootTask);
        SortedSet<string> sortedSet1 = new SortedSet<string>();
        SortedSet<string> sortedSet2 = new SortedSet<string>();
        SortedSet<string> sortedSet3 = new SortedSet<string>();
        foreach (TaskViewModel taskViewModel in taskViewModelList)
        {
          string type = taskViewModel.Type;
          if (!sortedSet1.Contains(type))
            sortedSet1.Add(type);
          string context = taskViewModel.Context;
          if (!sortedSet2.Contains(context))
            sortedSet2.Add(context);
          string title = taskViewModel.Title;
          if (!sortedSet3.Contains(title))
            sortedSet3.Add(title);
        }
        this.TypeList.Clear();
        foreach (string str in sortedSet1)
          this.TypeList.Add(str);
        this.ContextList.Clear();
        foreach (string str in sortedSet2)
          this.ContextList.Add(str);
        this.TaskTitleList.Clear();
        foreach (string str in sortedSet3)
          this.TaskTitleList.Add(str);
      }));
    }

    public async Task LoadConfigurationAsync()
    {
      this.Config = await XFile.ReadFromFileAsync<ConfigModel>(this.ConfigFile);
      if (this.Config == null)
        this.Config = new ConfigModel();
      this.LastListTaskId = this.Config.App.LastListTaskId;
      this.LastTreeTaskId = this.Config.App.LastTreeTaskId;
      this.Folder = this.Config.App.RecentFolder;
      this.RecentFile = this.Config.App.RecentFile;
      this.MostRecentFiles.Clear();
      if (!Directory.Exists(this.Folder))
        return;
      this.MostRecentFiles.Add(new FileInfo(this.Folder));
    }

    public async Task SaveConfigurationAsync()
    {
      if (this.Config == null)
        this.Config = new ConfigModel();
      this.Config.App.RecentFolder = this.Folder;
      this.Config.App.LastListTaskId = this.LastListTaskId;
      this.Config.App.LastTreeTaskId = this.LastTreeTaskId;
      this.Config.App.RecentFile = this.RecentFile;
      await XFile.SaveToFileAsync<ConfigModel>(this.Config, this.ConfigFile);
    }

    public void UseSettings()
    {
      this.Project.SelectTreeTask(this.LastListTaskId);
      this.Project.SelectTask(this.LastTreeTaskId);
    }

    public void PrepareSettings()
    {
      if (this.Project.SelectedTask != null)
        this.LastListTaskId = this.Project.SelectedTask.Id;
      if (this.Project.SelectedTreeTask == null)
        return;
      this.LastTreeTaskId = this.Project.SelectedTreeTask.Id;
    }

    public void SelectTreeTask(TaskViewModel task)
    {
      task.TypeList = this.TypeList;
      task.ContextList = this.ContextList;
      task.TaskTitleList = this.TaskTitleList;
      this.Project.SelectTreeTask(task);
      this.OnGenerateReportChanged();
    }

    public void CopyExcelCsvText()
    {
      StringReader stringReader = new StringReader(this.ExcelCsvText);
      List<ExcelCsvRecord> excelCsvRecordList = new List<ExcelCsvRecord>();
      string line;
      while ((line = stringReader.ReadLine()) != null)
      {
        if (!string.IsNullOrEmpty(line))
        {
          ExcelCsvRecord excelCsvRecord = new ExcelCsvRecord();
          if (excelCsvRecord.TryParse(line))
            excelCsvRecordList.Add(excelCsvRecord);
        }
      }
      TaskViewModel selectedTreeTask = this.Project.SelectedTreeTask;
      if (selectedTreeTask.Context != "Week")
        return;
      foreach (ExcelCsvRecord excelCsvRecord in excelCsvRecordList)
      {
        DateTime dateTime1 = excelCsvRecord.Day;
        string dayOfTheWeek = dateTime1.DayOfWeek.ToString();
        TaskViewModel taskViewModel1 = selectedTreeTask.SubTasks.FirstOrDefault<TaskViewModel>((Func<TaskViewModel, bool>) (t => t.Title == dayOfTheWeek));
        if (taskViewModel1 == null)
        {
          taskViewModel1 = new TaskViewModel(dayOfTheWeek, 0);
          taskViewModel1.DateStarted = excelCsvRecord.Day;
          taskViewModel1.DateEnded = excelCsvRecord.Day;
          selectedTreeTask.SubTasks.Add(taskViewModel1);
        }
        TaskViewModel taskViewModel2 = new TaskViewModel(excelCsvRecord.Task, 0);
        taskViewModel2.Context = "Task";
        TaskViewModel taskViewModel3 = taskViewModel2;
        dateTime1 = excelCsvRecord.Day;
        int year1 = dateTime1.Year;
        dateTime1 = excelCsvRecord.Day;
        int month1 = dateTime1.Month;
        dateTime1 = excelCsvRecord.Day;
        int day1 = dateTime1.Day;
        dateTime1 = excelCsvRecord.Start;
        int hour1 = dateTime1.Hour;
        dateTime1 = excelCsvRecord.Start;
        int minute1 = dateTime1.Minute;
        dateTime1 = excelCsvRecord.Start;
        int second1 = dateTime1.Second;
        DateTime dateTime2 = new DateTime(year1, month1, day1, hour1, minute1, second1);
        taskViewModel3.DateStarted = dateTime2;
        TaskViewModel taskViewModel4 = taskViewModel2;
        dateTime1 = excelCsvRecord.Day;
        int year2 = dateTime1.Year;
        dateTime1 = excelCsvRecord.Day;
        int month2 = dateTime1.Month;
        dateTime1 = excelCsvRecord.Day;
        int day2 = dateTime1.Day;
        dateTime1 = excelCsvRecord.End;
        int hour2 = dateTime1.Hour;
        dateTime1 = excelCsvRecord.End;
        int minute2 = dateTime1.Minute;
        dateTime1 = excelCsvRecord.End;
        int second2 = dateTime1.Second;
        DateTime dateTime3 = new DateTime(year2, month2, day2, hour2, minute2, second2);
        taskViewModel4.DateEnded = dateTime3;
        taskViewModel2.Description = string.Format("{0}:{1}:{2}", (object) excelCsvRecord.Type1, (object) excelCsvRecord.Type2, (object) excelCsvRecord.SubTask);
        taskViewModel1.SubTasks.Add(taskViewModel2);
      }
    }

    public async Task NewProjectAsync()
    {
      await this.SaveDataAsync();
      this.CanSave = false;
      this.Project.Clear();
      this.Project.RootTask.Add(new TaskViewModel("Time Tracker", 1)
      {
        Context = "Time Tracker"
      });
      this.Data = new Projects.Models.Versions.Version2.DataModel();
      this.Folder = string.Empty;
      this.CanSave = true;
    }

    public void OnTreeViewKeyDown(TaskViewModel.KeyStates keyState, TaskViewModel.KeyboardStates keyboardState)
    {
      TaskViewModel.OnTreeViewKeyDown(this.Project.SelectedTask, keyState, (Func<TaskViewModel.KeyboardStates>) (() => keyboardState), (Action) (() => {}), (Action<TaskViewModel>) (t => t.IsSelected = true), (Action<TaskViewModel>) (t => t.IsExpanded = true), (Func<bool>) (() => true), new Action<Action>(((BaseViewModel) this).OnDispatcher));
    }

    public void CopyTask()
    {
      this.OnTreeViewKeyDown(TaskViewModel.KeyStates.Insert, TaskViewModel.KeyboardStates.IsControlPressed);
    }

    public void ContinueTask()
    {
      this.OnTreeViewKeyDown(TaskViewModel.KeyStates.Insert, TaskViewModel.KeyboardStates.IsShiftPressed);
    }
  }
}
