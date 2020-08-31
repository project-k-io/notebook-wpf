using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace ProjectK.Notebook
{
    public class AppViewModel: MainViewModel
    {
        #region SaveDockLayoutCommand
        private ICommand _saveDockLayoutCommand;
        private ILogger _logger = null;
        private const string DockFileName = "DockStates.xml";

        public ICommand SaveDockLayoutCommand
        {
            get
            {
                _saveDockLayoutCommand = new DelegateCommand(SaveDockLayout, CanSelect);
                return _saveDockLayoutCommand;
            }
        }

        private bool CanSelect(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        public void SaveDockLayout(object obj)
        {
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;

            try
            {
                var writer = XmlWriter.Create(DockFileName);
                mainWindow.DockingManager.SaveDockState(writer);
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region LoadDockLayoutCommand
        private ICommand _loadDockLayoutCommand;

        public ICommand LoadDockLayoutCommand
        {
            get
            {
                _loadDockLayoutCommand = new DelegateCommand(LoadDockLayout, CanLoad);
                return _loadDockLayoutCommand;
            }
        }

        private bool CanLoad(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        public void LoadDockLayout(object obj)
        {
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;
            
            if(!File.Exists(DockFileName))
                return;

            try
            {
                var reader = XmlReader.Create(DockFileName);
                mainWindow.DockingManager.LoadDockState(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region AddCommand
        private ICommand _addCmd;

        public ICommand AddCommand
        {
            get
            {
                _addCmd = new DelegateCommand(Add, CanAdd);
                return _addCmd;
            }
        }

        private bool CanAdd(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        private void Add(object obj)
        {
            var count = 1;
            var contentControl = new ContentControl();
            contentControl.Name = "newChild" + count;
            DockingManager.SetHeader(contentControl, "New Child " + count);
            DockingManager.SetDesiredWidthInDockedMode(contentControl, 200);
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;
            mainWindow.DockingManager.Children.Add(contentControl);
            count++;
        }

        #endregion
    }
}
