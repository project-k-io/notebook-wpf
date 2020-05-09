﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace Vibor.View.Helpers.ViewModels
{
    public class OutputButtonViewModel : ViewModelBase
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
                RaisePropertyChanged(nameof(IsChecked));
            }
        }

        public int Image { get; set; }

        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public string Label { get; set; }

        public bool IsCountVisible { get; set; } = true;

        public ICommand ClickedCommand => new RelayCommand(OnClicked);

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