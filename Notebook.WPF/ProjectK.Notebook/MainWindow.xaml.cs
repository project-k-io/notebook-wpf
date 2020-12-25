using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectK.Logging;
using ProjectK.Notebook.Settings;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook
{
    public partial class MainWindow : Window
    {
        #region Consts

        private const string DockFileName = "DockStates.xml";

        #endregion

        private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();
        private readonly AppSettings _settings;

        public MainWindow(IOptions<AppSettings> settings)
        {
            InitializeComponent();
            _settings = settings.Value;

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

        public void LoadSettings()
        {
            var settings = _settings.Window;
            settings.SizeToFit();
            settings.MoveIntoView();

            Top = settings.Top;
            Left = settings.Left;
            Width = settings.Width;
            Height = settings.Height;
            WindowState = settings.WindowState;
        }

        public void SaveSettings()
        {
            var settings = _settings.Window;

            if (WindowState != WindowState.Minimized)
            {
                settings.Top = Top;
                settings.Left = Left;
                settings.Height = Height;
                settings.Width = Width;
                settings.WindowState = WindowState;
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
        ///     Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="window"></param>
        public void SaveDockLayout(MainWindow window)
        {
            try
            {
                var writer = XmlWriter.Create(DockFileName);
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
        ///     Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="window"></param>
        public void LoadDockLayout(MainWindow window)
        {
            if (!File.Exists(DockFileName))
                return;

            try
            {
                var reader = XmlReader.Create(DockFileName);
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