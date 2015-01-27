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
using PD.Manager.FramwWork;
using PD.Controls;
using PD.Controls.FrameWork;

namespace PD.Manager.Page.InitPage
{
    public partial class PDInitPage : UserControl, IDisposable
    {
        public PDInitPage()
        {
            SkinHelper.AddResource("/PD.Controls;component/Skin/Control/FrameworkTheme.xaml");
            InitializeComponent(); 
            btnQK.MouseLeftButtonUp +=    (SystemInQKClick);
            btnKB.MouseLeftButtonUp +=    (SystemInKBClick); 
        }

        /// <summary>
        /// 扣板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SystemInKBClick(object sender, MouseButtonEventArgs e)
        {
            InitSystem(SystemType.KB);
        }

        /// <summary>
        /// 墙扣
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SystemInQKClick(object sender, MouseButtonEventArgs e)
        {
            InitSystem(SystemType.QK);
        }

        private void InitSystem(SystemType type)
        {
            CurrentSystemType.Instance = new CurrentSystemType(type);
            PDMainPage.Main.InitLogin();
        }

        //void btnInSystem_Click(object sender, MouseButtonEventArgs e)
        //{
        //    MessageBox.Show("接口不开放");
        //    return;
        //    //PDMainPage.Main.InFrameWork();
        //}

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                App.Current.MainWindow.Close();
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Invoke("Close", null);
            }
        }

        public void Dispose()
        {
            btnQK.MouseLeftButtonUp -= (SystemInQKClick);
            btnKB.MouseLeftButtonUp -= (SystemInKBClick);
            //btnInSystem.MouseLeftButtonUp -= (btnInSystem_Click);
        }
    }

   
}
