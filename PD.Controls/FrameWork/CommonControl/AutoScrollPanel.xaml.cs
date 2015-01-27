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
using PD.ServiceClient.PDService;

namespace PD.Controls.FrameWork.CommonControl
{
    public partial class AutoScrollPanel : UserControl, IDisposable
    {
        public event EventHandler MessageClick;

        public List<T_PB_NewList> ScrollContent
        {
            get { return (List<T_PB_NewList>)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        const int height = 31;

        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
        "ScrollContent",
        typeof(List<T_PB_NewList>),
        typeof(AutoScrollPanel),
        new PropertyMetadata(DisplayDateChanged));

        private static void DisplayDateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                (o as AutoScrollPanel).panel.Children.ToList().ForEach(item =>
                {
                    (item as HyperlinkButton).Click -= (o as AutoScrollPanel).ContentMessageClick;
                });
                (o as AutoScrollPanel).panel.Children.Clear();
                int i = 0;
                (e.NewValue as List<T_PB_NewList>).ForEach(item =>
                {
                    var c = new HyperlinkButton { FontSize = 20, DataContext = item, Foreground = new SolidColorBrush { Color = Colors.Black }, Content = item.MsgTitle, Margin = new Thickness(0, 0, 0, 0) };
                    c.Height = height;
                    c.Click += (o as AutoScrollPanel).ContentMessageClick;
                    (o as AutoScrollPanel).panel.Children.Add(c);
                    i++;
                    if (i <= 2)
                    {
                        var cCopy = new HyperlinkButton { FontSize = 20, DataContext = item, Foreground = new SolidColorBrush { Color = Colors.Black }, Content = item.MsgTitle, Margin = new Thickness(0, 0, 0, 0) };
                        cCopy.Height = height;
                        cCopy.Click += (o as AutoScrollPanel).ContentMessageClick;
                        (o as AutoScrollPanel).panelCopy.Children.Add(cCopy);
                    }
                });
                Canvas.SetTop((o as AutoScrollPanel).panelCopy, (e.NewValue as List<T_PB_NewList>).Count * height);
            }
        }

        void ContentMessageClick(object sender, RoutedEventArgs e)
        {
            timer.Start();
            if (MessageClick != null)
            {
                MessageClick((sender as FrameworkElement).DataContext, EventArgs.Empty);
            }
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 2000)
        };

        System.Windows.Threading.DispatcherTimer timerMove = new System.Windows.Threading.DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 50)
        };

        public AutoScrollPanel()
        {
            InitializeComponent();
            panel.MouseLeave += (panel_MouseLeave);
            panel.MouseEnter += (panel_MouseEnter);
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                timer.Tick += (timer_Tick);
                timerMove.Tick += (timerMove_Tick);
                timer.Start();
            }
        }
        int i = 0;
        void timerMove_Tick(object sender, EventArgs e)
        {
            i++;
            if (i > height)
            {
                timerMove.Stop();
                i = 0;
                return;
            }
            if (Math.Abs(Canvas.GetTop(panel)) > panel.ActualHeight)
            {
                Canvas.SetTop(panel, -1);
                Canvas.SetTop(panelCopy, panel.ActualHeight - 1);
            }
            Canvas.SetTop(panel, Canvas.GetTop(panel) - 1);
            Canvas.SetTop(panelCopy, Canvas.GetTop(panelCopy) - 1);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timerMove.Start();
        }

        void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Start();
        }

        void panel_MouseEnter(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }

        public void Dispose()
        {
            panel.MouseLeave -= (panel_MouseLeave);
            panel.MouseEnter -= (panel_MouseEnter);
            panelCopy.MouseLeave -= (panel_MouseLeave);
            panelCopy.MouseEnter -= (panel_MouseEnter);
            timer.Stop();
            timerMove.Stop();
            timerMove.Tick -= (timerMove_Tick);
            timer.Tick -= (timer_Tick);
            panel.Children.ToList().ForEach(item =>
           {
               (item as HyperlinkButton).Click -= ContentMessageClick;
           }); this.panelCopy.Children.ToList().ForEach(item =>
          {
              (item as HyperlinkButton).Click -= ContentMessageClick;
          });
        }
    }
}
