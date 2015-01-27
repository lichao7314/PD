using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PD.Controls
{
    public partial class DataGridSelectColumnHelper : UserControl
    {

        internal DataGridSelectColumn SelectColumn { get; set; }
        public DataGridSelectColumnHelper()
        {
            InitializeComponent();
            this.MarkObject = this.Resources["MarkObject"] as MarkObject;
        }

        public MarkObject MarkObject { get; private set; }

        private void ckbSelected_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as CheckBox;
            if (!(element.DataContext is  MarkObject))
            {
                var data = ((System.Windows.Controls.DataGridCell)(((System.Windows.Controls.Primitives.ToggleButton)(sender)).Parent)).DataContext;
                element.DataContext = SelectColumn.GetMarkObject(data);
                element.IsChecked = SelectColumn.GetMarkObject(data).Selected;
            }
        }
    }
}
