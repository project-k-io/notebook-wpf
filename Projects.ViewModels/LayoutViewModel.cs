using GalaSoft.MvvmLight;

namespace Projects.ViewModels
{
    public class LayoutViewModel : ViewModelBase
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
                RaisePropertyChanged();
            }
        }
    }
}