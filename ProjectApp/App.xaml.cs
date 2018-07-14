// Decompiled with JetBrains decompiler
// Type: ProjectApp.App
// Assembly: ProjectApp, Version=1.1.8.29131, Culture=neutral, PublicKeyToken=null
// MVID: A7331AD2-AF8A-4A84-BF9D-60C36001D1E0
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\ProjectApp.exe

using ProjectApp.Properties;
using Projects.Models.Versions.Version2;
using Projects.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Vibor.Logging;

namespace ProjectApp
{
  public class App : Application
  {
    private static readonly ILog Logger = LogManager.GetLogger("Converter");
    private readonly MainWindow _mainWindow = new MainWindow();
    private readonly MainViewModel _mainModel = new MainViewModel();
    private bool _canSave;

    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      await this._mainModel.LoadDataAsync();
      await this._mainModel.UpdateTypeListAsync();
      this.LoadSettings(this._mainModel.Config.Layout);
      this._mainWindow.DataContext =  this._mainModel;
      this._mainWindow.Show();
      await this.StartSavingAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);
      this.SaveSettings(this._mainModel.Config.Layout);
      this.StopSaving();
      await this._mainModel.SaveDataAsync();
      this._mainWindow.Close();
      this.Shutdown();
    }

    public async Task StartSavingAsync()
    {
      this._canSave = true;
      while (this._canSave)
      {
        this.SaveSettings(this._mainModel.Config.Layout);
        await this._mainModel.SaveDataAsync();
        await Task.Run((Action) (() => Thread.Sleep(5000)));
      }
    }

    public void StopSaving()
    {
      this._canSave = false;
    }

    private void LoadSettings(ConfigModel.LayoutModel s1)
    {
      // ISSUE: variable of a compiler-generated type
      Settings settings = Settings.Default;
      try
      {
        this._mainWindow.WindowState = settings.MainWindowState;
        this._mainWindow.Top = s1.MainWindowTop;
        this._mainWindow.Left = s1.MainWindowLeft;
        this._mainWindow.Width = s1.MainWindowWidth;
        this._mainWindow.Height = s1.MainWindowHeight;
      }
      catch (Exception ex)
      {
        App.Logger.Error( ex);
      }
      this._mainModel.Layout.NavigatorWidth = s1.LayoutNavigatorWidth;
    }

    private void SaveSettings(ConfigModel.LayoutModel s1)
    {
      try
      {
        this._mainModel.PrepareSettings();
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        if (this._mainWindow.WindowState != WindowState.Minimized)
        {
          s1.MainWindowTop = this._mainWindow.Top;
          s1.MainWindowLeft = this._mainWindow.Left;
          s1.MainWindowWidth = this._mainWindow.Width;
          s1.MainWindowHeight = this._mainWindow.Height;
          s1.LayoutNavigatorWidth = this._mainModel.Layout.NavigatorWidth;
        }
        settings.MainWindowState = this._mainWindow.WindowState;
        settings.Save();
      }
      catch (Exception ex)
      {
        App.Logger.Error( ex);
      }
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    [STAThread]
    public static void Main()
    {
      new App().Run();
    }
  }
}
