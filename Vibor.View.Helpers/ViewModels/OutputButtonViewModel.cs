// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.OutputButtonViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.Windows.Input;
using Vibor.Mvvm;

namespace Vibor.Generic.ViewModels
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