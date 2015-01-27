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
using System.Windows.Controls.Primitives;
 

namespace PD.Controls
{
    public partial class MonthDatePicker : UserControl, IDisposable
    {
        public string Month
        {
            get
            {
                return calendarControl.DisplayDate.ToString(formatDate);
            }
        }
        public DateTime DisplayDate
        {
            get { return (DateTime)calendarControl.DisplayDate; }
            set { SetValue(DisplayDateProperty, value); }
        }
        public event EventHandler<MonthDataChangeArgs> DateChangedEvent;
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
        "DisplayDate",
        typeof(DateTime),
        typeof(MonthDatePicker),
        new PropertyMetadata(DateTime.Now, DisplayDateChanged));

        private static void DisplayDateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (o as MonthDatePicker).calendarControl.DisplayDate = (DateTime)e.NewValue;
                (o as MonthDatePicker).SetDropValue();
            }
        }

        private string formatDate = "yyyy-MM";

        private Popup calendarPopup = null;

        private Calendar calendarControl = null;

        // 摘要:
        //     当关闭组合框的下拉部分时发生。
        public event EventHandler DropDownClosed;
        //
        // 摘要:
        //     当打开组合框的下拉部分时发生。
        public event EventHandler DropDownOpened;

        public MonthDatePicker()
        {
            InitializeComponent();
            calendarPopup = new Popup();
            calendarControl = new Calendar() { Name = "Calendar1", DisplayMode = CalendarMode.Decade };
            calendarPopup.Child = new Grid()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Width = 2000,
                Height = 2000
            };
            (calendarPopup.Child as Grid).Children.Add(calendarControl);
            (calendarPopup.Child as Grid).MouseLeftButtonUp += new MouseButtonEventHandler(MonthDatePicker_MouseLeftButtonUp);
            (calendarPopup.Child as Grid).Background = new SolidColorBrush() { Color = Colors.Transparent };
            calendarControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            calendarControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            calendarControl.DisplayDateChanged += CalendarDisplayDateChanged;
            calendarControl.DisplayModeChanged += CalendarDisplayModeChanged;
            dateDropContent.DropDownOpened += DateDropDownOpened;
        }

        private void MonthDatePicker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            calendarPopup.IsOpen = false;
            if (DropDownClosed != null)
                DropDownClosed(this, EventArgs.Empty);
        }

        private void CalendarDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            calendarControl.DisplayModeChanged -= CalendarDisplayModeChanged;
            if (calendarControl.DisplayMode == CalendarMode.Month)
            {
                dateDropContent.IsDropDownOpen = false;
                SetDropValue();
                calendarControl.DisplayMode = CalendarMode.Year;
                calendarPopup.IsOpen = false;
                if (DateChangedEvent != null)
                {
                    DateChangedEvent(this, new MonthDataChangeArgs
                    {
                        Month = Month,
                        DisplayDate = DisplayDate
                    });
                }
                if (DropDownClosed != null)
                    DropDownClosed(this, EventArgs.Empty);
            }
            calendarControl.DisplayModeChanged += CalendarDisplayModeChanged;
        }

        private void CalendarDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            SetDropValue();
        }

        private void SetDropValue()
        {
            dateDropContent.Items.Clear();
            dateDropContent.Items.Add(calendarControl.DisplayDate.ToString(formatDate));
            dateDropContent.SelectedIndex = 0;
            this.DisplayDate = calendarControl.DisplayDate;
        }

        private void DateDropDownOpened(object sender, EventArgs e)
        {
            calendarControl.DisplayMode = CalendarMode.Year;
            ComboBox tb = sender as ComboBox;
            var gt = tb.TransformToVisual(null);
            Point p = gt.Transform(new Point(0, 0));
            calendarControl.Margin = new Thickness(p.X, p.Y + this.dateDropContent.ActualHeight, 0, 0);
            calendarPopup.IsOpen = true;
            dateDropContent.IsDropDownOpen = false;
            if (DropDownOpened != null)
                DropDownOpened(this, EventArgs.Empty);
        }
      
        public void Dispose()
        {
            (calendarPopup.Child as Grid).MouseLeftButtonUp -= new MouseButtonEventHandler(MonthDatePicker_MouseLeftButtonUp);
            calendarControl.DisplayDateChanged -= CalendarDisplayDateChanged;
            calendarControl.DisplayModeChanged -= CalendarDisplayModeChanged;
            dateDropContent.DropDownOpened -= DateDropDownOpened;
        }
    }

    public class MonthDataChangeArgs : DataChangeArgs
    {
        public string Month { get; set; }

    }
}
