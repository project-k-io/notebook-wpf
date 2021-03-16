using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ProjectK.Notebook.WinApp
{
    public class LayoutViewModel : ViewModelBase
    {
        private int _outputHeight = 400;
        private int _navigatorWidth = 200;
        private int _propertiesWidth = 400;

        public int OutputHeight { get => _outputHeight; set => Set(ref _outputHeight, value); }
        public int NavigatorWidth { get => _navigatorWidth; set => Set(ref _navigatorWidth, value); }
        public int PropertiesWidth { get => _propertiesWidth; set => Set(ref _propertiesWidth, value); }

    }
}
