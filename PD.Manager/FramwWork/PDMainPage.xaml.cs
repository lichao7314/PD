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
using PD.ServiceClient;
using System.Net.NetworkInformation;
using PD.Manager.Page.InitPage;

namespace PD.Manager.FramwWork
{
    public partial class PDMainPage : UserControl, IModule
    {
        public static PDMainPage Main
        {
            get;
            set;
        }

        public PDMainPage()
        {
            InitializeComponent();
            Main = this;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                WcfConfigs.Instance.Initialize();
                WcfConfigs.Instance.InitializeCompleted += (Instance_InitializeCompleted);
            }
            else
            {
                Init();
            }
        }

        void Instance_InitializeCompleted(object sender, EventArgs e)
        {
            WcfConfigs.Instance.InitializeCompleted -= (Instance_InitializeCompleted);
            Init();
        }

        public void InitLogin()
        {
            var login = new PDLogin();
            login.LoginCompleted += (login_LoginCompleted);
            this.Content = login;
            HiddenMouseRightKey(Content);
        }

        void login_LoginCompleted(object sender, EventArgs e)
        {
            new ServerTimeServer().GetTime();
            (sender as IDisposable).Dispose();
            InFrameWork();
        }

        public void Init()
        {
            if (this.Content is IDisposable) {
                (this.Content as IDisposable).Dispose();
            }
            this.Content = new PDInitPage();
        }

        public void InFrameWork()
        {
            if (this.Content is IDisposable)
            {
                (this.Content as IDisposable).Dispose();
            }
            var fr = new PDFrameWork();
            fr.LogoutSystem += (fr_LogoutSystem);
            this.Content = fr;
            HiddenMouseRightKey(Content);
        }

        void fr_LogoutSystem(object sender, EventArgs e)
        {
            (sender as PDFrameWork).Dispose();
            Init();
        }

        /// <summary>
        /// 屏蔽右键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            //e.Handled = true;
        }

        void PDMainPage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void HiddenMouseRightKey(UIElement content)
        {
            content.MouseRightButtonDown += (PDMainPage_MouseLeftButtonDown);
            content.MouseRightButtonUp += PDMainPage_MouseLeftButtonDown;
        }

        public void RegisterEvent()
        {
        }

        public void UnRegisterEvent()
        {
        }

        public void Dispose()
        {
        }
    }
}
