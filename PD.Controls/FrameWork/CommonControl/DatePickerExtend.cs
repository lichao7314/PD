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
using System.Windows.Controls.Primitives;

namespace PD.Controls.FrameWork
{
    public class DatePickerExtend : DatePicker
    {
        DateTime FirstDateTime;
        public bool AllowCleanDate { get; set; }

        public DatePickerExtend()
            : base()
        {
            this.CalendarClosed += new RoutedEventHandler(DatePickerExtend_CalendarClosed);
        }

        void DatePickerExtend_CalendarClosed(object sender, RoutedEventArgs e)
        {
            FirstDateTime = this.SelectedDate.Value;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.SelectedDate == null)
            {
                this.SelectedDate = DateTime.Now;
            }
            FirstDateTime = this.SelectedDate.Value;
            DatePickerTextBox _DatePickerTextBox = GetTemplateChild("TextBox") as DatePickerTextBox;
            _DatePickerTextBox.TextChanged += new TextChangedEventHandler(_DatePickerTextBox_TextChanged);
            if (AllowCleanDate)
            {
                _DatePickerTextBox.IsReadOnly = false;
            }
            else
            {
                _DatePickerTextBox.IsReadOnly = true;
            }
        }

        void _DatePickerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatePickerTextBox _DatePickerTextBox = sender as DatePickerTextBox;
            if (string.IsNullOrEmpty(_DatePickerTextBox.Text))
            {
                _DatePickerTextBox.Text = FirstDateTime.ToString("yyyy-MM-dd");
            }
        }
    }
}
