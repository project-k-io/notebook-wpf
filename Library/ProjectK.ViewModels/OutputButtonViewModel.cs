using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ProjectK.ViewModels
{
    public class OutputButtonViewModel : ViewModelBase
    {
        private int _count;
        private bool _isChecked = true;

        public bool IsChecked
        {
            get => _isChecked;
            set => Set(ref _isChecked, value);
        }


        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public string Label { get; set; }

        public bool IsCountVisible { get; set; } = true;

        public ICommand ClickedCommand => new RelayCommand(OnClicked);


        public string Image { get; set; }

        public event EventHandler Clicked;

        private void OnClicked()
        {
            var clicked = Clicked;
            clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}