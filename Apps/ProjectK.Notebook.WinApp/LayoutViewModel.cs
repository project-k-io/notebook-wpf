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
        private int _outputHeight = 200;

        public int OutputHeight { get => _outputHeight; set => Set(ref _outputHeight, value); }

    }
}
