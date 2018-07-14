using System;
using System.Windows.Input;
using Vibor.Mvvm;

namespace Vibor.View.Helpers.ViewModels
{
    public class OutputButtonViewModel : BaseViewModel
    {
        private int _count;
        private bool _isChecked = true;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public int Image { get; set; }

        public int Count
        {
            get => _count;
            set
            {
                if (_count == value)
                    return;
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        public string Label { get; set; }

        public bool IsCountVisible { get; set; } = true;

        public ICommand ClickedCommand => new RelayCommand(OnClicked, null);

        public event EventHandler Clicked;

        private void OnClicked()
        {
            var clicked = Clicked;
            if (clicked == null)
                return;
            clicked(this, EventArgs.Empty);
        }
    }
}