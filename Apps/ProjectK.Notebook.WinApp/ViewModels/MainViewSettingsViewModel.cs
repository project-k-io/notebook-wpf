using GalaSoft.MvvmLight;
using ProjectK.Notebook.WinApp.Models;

namespace ProjectK.Notebook.WinApp.ViewModels
{
    public class MainViewSettingsViewModel : ViewModelBase
    {
        private int _outputHeight = 400;
        private int _navigatorWidth = 200;
        private int _propertiesWidth = 400;

        public int OutputHeight { get => _outputHeight; set => Set(ref _outputHeight, value); }
        public int NavigatorWidth { get => _navigatorWidth; set => Set(ref _navigatorWidth, value); }
        public int PropertiesWidth { get => _propertiesWidth; set => Set(ref _propertiesWidth, value); }

        public MainViewSettingsModel Model
        {
            get => new()
            {
                NavigatorWidth = _navigatorWidth,
                PropertiesWidth = _propertiesWidth, 
                OutputHeight = _outputHeight
            };
            set
            {
                NavigatorWidth = value.NavigatorWidth;
                PropertiesWidth = value.PropertiesWidth;
                OutputHeight = value.OutputHeight;
            }
        }

    }
}
