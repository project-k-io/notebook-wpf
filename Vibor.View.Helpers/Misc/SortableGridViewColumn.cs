using System.Windows;
using System.Windows.Controls;

namespace Vibor.View.Helpers.Misc
{
    public class SortableGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(nameof(FieldName),
            typeof(string), typeof(SortableGridViewColumn), new UIPropertyMetadata(""));

        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.Register(nameof(IsDefaultSortColumn), typeof(bool), typeof(SortableGridViewColumn),
                new UIPropertyMetadata(false));

        public string FieldName
        {
            get => (string) GetValue(FieldNameProperty);
            set => SetValue(FieldNameProperty, value);
        }

        public bool IsDefaultSortColumn
        {
            get => (bool) GetValue(IsDefaultSortColumnProperty);
            set => SetValue(IsDefaultSortColumnProperty, value);
        }
    }
}