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
using PD.Controls;

namespace PD.Controls
{
    /// <summary>
    /// 日志记录弹出窗口
    /// </summary>
    public partial class PopupExceptionWindow : ChildWindow
    {
        public PopupExceptionWindow()
        {
            InitializeComponent();
            this.MinWidth = 200;
            this.MaxWidth = 500;
            this.LayoutRoot_R1C0.Height = new GridLength(0);//︾︽
            //this.txtMessage.MaxWidth = 500;
            this.richTxtMessage.MaxWidth = 500;
            this.richTxtMessage.MaxHeight = 100;

            //this.txtStack.MaxWidth = 500;
            //this.txtStack.Visibility = Visibility.Collapsed;
            this.richTxtStack.MaxWidth = 500;
            this.richTxtStack.MaxHeight = 200;
            this.richTxtStack.Visibility = Visibility.Collapsed;
            this.richTxtStack.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;


            System.Windows.Documents.Run run1 = new Run();
            run1.Text = "";
            Paragraph Paragraph1 = new Paragraph();
            Paragraph1.Inlines.Add(run1);
            this.richTxtMessage.Blocks.Add(Paragraph1);

            run1 = new Run();
            run1.Text = "";
            Paragraph1 = new Paragraph();
            Paragraph1.Inlines.Add(run1);
            this.richTxtStack.Blocks.Add(Paragraph1);
        }

        //public string rMessage;
        //public string rStackTrace;    


        /// <summary>
        /// 显示日志记录弹出窗口
        /// </summary>
        /// <param name="ex">异常变量</param>
        /// <param name="ClosedMethod">窗口关闭事件</param>
        public static void ShowExceptionWindow(System.Exception ex, EventHandler ClosedMethod)
        {
            System.Exception ex1 = ExceptionHelper.GetInnerException(ex);
            ShowExceptionWindow("信息", ex1.Message, "内部信息\r\n" + ex1.StackTrace, ClosedMethod);
        }

        /// <summary>
        /// 显示日志记录弹出窗口
        /// </summary>
        /// <param name="ex">异常变量</param>
        public static void ShowExceptionWindow(System.Exception ex)
        {
            ShowExceptionWindow(ex);
        }

        /// <summary>
        /// 显示日志记录弹出窗口
        /// </summary>
        /// <param name="vTitle">窗口标签</param>
        /// <param name="vMessage">消息</param>
        /// <param name="vStackTrace">日志异常代码调试信息</param>
        /// <param name="ClosedMethod">窗口关闭事件</param>
        public static void ShowExceptionWindow(string vTitle, string vMessage, string vStackTrace, EventHandler ClosedMethod)
        {
            PopupExceptionWindow exWindow = new PopupExceptionWindow();
            exWindow.Title = vTitle;
            ((Run)((Paragraph)exWindow.richTxtMessage.Blocks[0]).Inlines[0]).Text = vMessage;
            ((Run)((Paragraph)exWindow.richTxtStack.Blocks[0]).Inlines[0]).Text = vStackTrace;
            exWindow.Closed += ClosedMethod;
            exWindow.Show();
        }

        /// <summary>
        /// 显示日志记录弹出窗口
        /// </summary>
        /// <param name="vTitle">窗口标签</param>
        /// <param name="vMessage">消息</param>
        /// <param name="vStackTrace">日志异常代码调试信息</param>
        public static void ShowExceptionWindow(string vTitle, string vMessage, string vStackTrace)
        {
            ShowExceptionWindow(vTitle, vMessage, vStackTrace, null);
        }



        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        double? rrichTxtStackHeight = null;
        private void btnShowStack_Click(object sender, RoutedEventArgs e)
        {
            if (System.Convert.ToString(this.btnShowStack.Content) == "▶")
            {
                this.LayoutRoot_R1C0.Height = new GridLength(30, GridUnitType.Auto);
                this.btnShowStack.Content = "◀";
                //this.txtStack.Visibility = Visibility.Visible;
                this.richTxtStack.ActualHeight.ToString();
                this.richTxtStack.Visibility = Visibility.Visible;
                this.richTxtStack.UpdateLayout();
                if (rrichTxtStackHeight == null)
                {
                    rrichTxtStackHeight = this.richTxtStack.ActualHeight;
                }

                DoubleAnimation dAnimation = new DoubleAnimation();
                dAnimation.From = 0;
                dAnimation.To = rrichTxtStackHeight;
                dAnimation.Duration = TimeSpan.Parse("00:00:00.5");



                Storyboard sbBoard = new Storyboard();
                Storyboard.SetTarget(dAnimation, this.richTxtStack);
                Storyboard.SetTargetProperty(dAnimation, new PropertyPath(RichTextBox.HeightProperty));
                sbBoard.Children.Add(dAnimation);
                sbBoard.Begin();

                //this.richTxtStack.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.LayoutRoot_R1C0.Height = new GridLength(0, GridUnitType.Auto);
                this.btnShowStack.Content = "▶";
                //this.txtStack.Visibility = Visibility.Collapsed;
                //this.richTxtStack.Visibility = Visibility.Collapsed;

                DoubleAnimation dAnimation = new DoubleAnimation();
                dAnimation.From = rrichTxtStackHeight;
                dAnimation.To = 0;
                dAnimation.Duration = TimeSpan.Parse("00:00:00.5");

                Storyboard sbBoard = new Storyboard();
                Storyboard.SetTarget(dAnimation, this.richTxtStack);
                Storyboard.SetTargetProperty(dAnimation, new PropertyPath(RichTextBox.HeightProperty));
                sbBoard.Children.Add(dAnimation);
                sbBoard.Begin();
            }
        }


    }




    
}

