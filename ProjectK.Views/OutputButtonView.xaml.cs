using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectK.Views
{
    /// <summary>
    /// Interaction logic for OutputButtonView.xaml
    /// </summary>
    public partial class OutputButtonView : UserControl
    {
        public OutputButtonView()
        {
            InitializeComponent();
        }

        public string Image
        {
            get => (string)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(string), typeof(OutputButtonView), new PropertyMetadata(false));
    }
}
