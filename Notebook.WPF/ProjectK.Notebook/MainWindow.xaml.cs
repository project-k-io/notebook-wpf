 using System;
 using System.Collections.Specialized;
 using System.Configuration;
 using System.Globalization;
 using System.IO;
 using System.Windows;
using System.Windows.Controls;
 using System.Xml;
 using GalaSoft.MvvmLight.Command;
 using Microsoft.Extensions.Logging;
using ProjectK.Logging;
 using ProjectK.Notebook.Extensions;
 using ProjectK.View.Helpers.Misc;
 using Syncfusion.Windows.Tools.Controls;
 using Calendar = System.Windows.Controls.Calendar;

 namespace ProjectK.Notebook
 {
     public partial class MainWindow : Window
     {
         private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();

         #region Consts

         private const string DockFileName = "DockStates.xml";

         #endregion

        public MainWindow()
         {
             InitializeComponent();
             Loaded += MainView_Loaded;
         }

         private void MainView_Loaded(object sender, RoutedEventArgs e)
         {
             _logger.LogDebug("Loaded()");
             if (!(DataContext is AppViewModel model)) return;

             model.LoadDockLayoutCommand = new RelayCommand(LoadDockLayout);
             model.SaveDockLayoutCommand = new RelayCommand(SaveDockLayout);

            model.OnDispatcher = ViewLib.GetAddDelegate(this);
             CommandBindings.AddRange(model.CreateCommandBindings());
         }

         private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
         {
             if (!(DataContext is AppViewModel model)) return;
             if (!(sender is Calendar calendar)) return;
             model.SelectedNotebook.UpdateSelectDayTasks(calendar.SelectedDates);
             model.OnGenerateReportChanged();
         }




         private void DockingManager_OnDockStateChanged(FrameworkElement sender, DockStateEventArgs e)
         {
             if (e.NewState == DockState.Hidden)
             {
                 DockingManager.Children.Remove(sender);
             }
         }


         #region DockingManager

         public void SaveDockLayout()
         {
             if (!(Application.Current.MainWindow is MainWindow window))
                 return;

             SaveDockLayout(window);
         }


         /// <summary>
         /// Helps to perform save and load operation of Docking Manager.
         /// </summary>
         /// <param name="window"></param>
         public void SaveDockLayout(MainWindow window)
         {
             try
             {
                 var writer = XmlWriter.Create(DockFileName);
                 window.DockingManager.SaveDockState(writer);
                 writer.Close();
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 throw;
             }
         }

         public void LoadDockLayout()
         {
             if (!(Application.Current.MainWindow is MainWindow window))
                 return;

             LoadDockLayout(window);
         }

         /// <summary>
         /// Helps to perform save and load operation of Docking Manager.
         /// </summary>
         /// <param name="window"></param>
         public void LoadDockLayout(MainWindow window)
         {
             if (!File.Exists(DockFileName))
                 return;

             try
             {
                 var reader = XmlReader.Create(DockFileName);
                 window.DockingManager.LoadDockState(reader);
                 reader.Close();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex.Message);
                 throw;
             }
         }
         #endregion
     }
}