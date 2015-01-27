using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PD.Controls 
{
    public class ElementOrientationProvider : ControlOrientationProvider
    {
        protected override void HandleOwnerMouseRightButtonDown(object sender, KeyEventArgs e)
        {
            base.HandleOwnerMouseRightButtonDown(sender, e);

            if (e.Key == Key.Up)
            {
                Behavior.DataGrid.SelectedItem = Behavior.DataGrid.LastRowItem();
            }
            if (e.Key == Key.Down)
            {
                Behavior.DataGrid.SelectedItem = Behavior.DataGrid.NextRowItem();
            }
        }
    }
}
