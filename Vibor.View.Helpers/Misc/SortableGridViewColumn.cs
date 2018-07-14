// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.SortableGridViewColumn
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

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