using System.Windows;
using System.Windows.Controls;

namespace ProjectK.Notebook.Views.Items
{
    /// <summary>
    /// Interaction logic for PropertiesView.xaml
    /// </summary>
    public partial class ItemView : UserControl
    {
        public ItemView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void ItemViewList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
