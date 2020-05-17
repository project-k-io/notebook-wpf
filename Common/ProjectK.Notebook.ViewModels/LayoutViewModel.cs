using GalaSoft.MvvmLight;

namespace ProjectK.Notebook.ViewModels
{
    public class LayoutViewModel : ViewModelBase
    {
        private int _navigatorWidth;

        public int NavigatorWidth
        {
            get => _navigatorWidth;
            set => Set(ref _navigatorWidth, value);
        }
    }
}