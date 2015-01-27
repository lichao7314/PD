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
using Infotech.MES.Controls;
namespace PD.Controls
{
    public class TextBoxOrientationProvider : ControlOrientationProvider
    {
        private TextBox textBox;

        public override void SetOrientationProvider(FrameworkElement control)
        {
            if (control is TextBox)
            {
                var textBox = control as TextBox;

                this.textBox = textBox;

                textBox.Loaded += (textBox_Loaded);

                base.SetOrientationProvider(control);
            }
        }

        void textBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocus();
        }

        public override void SetFocus()
        {
            bool focus = textBox.Focus();
        }

        public override void Dispose()
        {
            textBox.Loaded -= (textBox_Loaded);
            base.Dispose();
        }
    }
}
