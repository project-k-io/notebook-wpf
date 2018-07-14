using Vibor.Mvvm;

namespace Projects.ViewModels
{
    public class LayoutViewModel : BaseViewModel
    {
        private int _navigatorWidth;

        public int NavigatorWidth
        {
            get => _navigatorWidth;
            set
            {
                if (_navigatorWidth == value)
                    return;
                _navigatorWidth = value;
                OnPropertyChanged(nameof(NavigatorWidth));
            }
        }
    }
}