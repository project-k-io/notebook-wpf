using System.Windows.Controls;

namespace ProjectK.Notebook.Views.Items
{
    public partial class TaskView : UserControl
    {
        public TaskView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // ((System.Windows.FrameworkElement)((System.Windows.FrameworkElement)this.TemplatedParent).TemplatedParent).DataContext
            // ().DataContext
            var fe1 = (System.Windows.FrameworkElement) this.TemplatedParent;
            var fe2 = (System.Windows.FrameworkElement) fe1.TemplatedParent;
            var dc = fe2.DataContext;
        }
    }
}