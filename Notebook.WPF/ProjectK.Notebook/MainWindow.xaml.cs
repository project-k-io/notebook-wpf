 using System.Collections.Specialized;
 using System.Configuration;
 using System.Globalization;
 using System.Windows;
using System.Windows.Controls;
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

         public MainWindow()
         {
             InitializeComponent();
             Loaded += MainView_Loaded;
         }

         private void MainView_Loaded(object sender, RoutedEventArgs e)
         {
             _logger.LogDebug("Loaded()");
             if (!(DataContext is AppViewModel model)) return;

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

         public void LoadSettings(NameValueCollection appSettings)
         {
             // window settings
             WindowState = appSettings.GetEnumValue("MainWindowState", WindowState.Normal);
             Top = appSettings.GetDouble("MainWindowTop", 100);
             Left = appSettings.GetDouble("MainWindowLeft", 100);
             Width = appSettings.GetDouble("MainWindowWidth", 800);
             Height = appSettings.GetDouble("MainWindowHeight", 400d);
         }

         public void SaveSettings(KeyValueConfigurationCollection settings)
         {
             // ISSUE: variable of a compiler-generated type
             // window settings
             if (WindowState != WindowState.Minimized)
             {
                 settings.SetValue("MainWindowTop", Top.ToString(CultureInfo.InvariantCulture));
                 settings.SetValue("MainWindowLeft", Left.ToString(CultureInfo.InvariantCulture));
                 settings.SetValue("MainWindowWidth", Width.ToString(CultureInfo.InvariantCulture));
                 settings.SetValue("MainWindowHeight", Height.ToString(CultureInfo.InvariantCulture));
             }
         }
     }
 }