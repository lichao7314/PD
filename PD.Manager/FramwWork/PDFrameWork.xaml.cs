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
using PD.ServiceClient.PDService;
using PD.ServiceClient;
using System.Reflection;
using PD.Controls.FrameWork;
using System.Windows.Media.Imaging;

namespace PD.Manager.FramwWork
{
    public partial class PDFrameWork : UserControl, IModule
    {
        public static PDFrameWork CurrentFrameWork { get; set; }

        public event EventHandler LogoutSystem;

        private PDServiceClient client = new ClientProxy().Client;

        public BaseModulePage CurrentModulePage { get; private set; }

        private int vailUser = 0;//1个小时验证下用户信息


        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 20, 0, 0)
        };

        public PDFrameWork()
        {
            InitializeComponent();
            RegisterEvent();
            CurrentFrameWork = this;
            timer.Start();

            Storyboard1.Begin();

            linkImage.TargetName = "_blank";
            if (CurrentSystemType.Instance.SystemType == 0)
            {
                imgType.Source = new BitmapImage(new Uri("/Images/FrameWork/集成墙面.png", UriKind.Relative));
                linkImage.NavigateUri = new Uri("http://www.tscnqm.com", UriKind.Absolute);
            }
            else
            {
                imgType.Source = new BitmapImage(new Uri("/Images/FrameWork/智能集成吊顶.png", UriKind.Relative));
                linkImage.NavigateUri = new Uri("http://www.tscndd.com", UriKind.Absolute);
                 
            }

            tbUserName.Text = (App.Current as App).Profile.UserName;

            btnDownLoad.NavigateUri = CommonMethod.FileNetAddress("/APK/TSKN.apk");

            InitMenu();
        }

        #region 定时更新
        void UpdateInTick(object sender, EventArgs e)
        {

            vailUser++;

            if (vailUser == 3)
            {
                client.UserLoginAsync((App.Current as IApplicationFramework).Profile.UserCode,
                    (App.Current as IApplicationFramework).Profile.PassWord, CurrentSystemType.Instance.SystemType);
                vailUser = 0;
            }
        }

      
        #endregion

        #region 验证用户信息
        void UserLoginCompleted(object sender, UserLoginCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                VailLogin(e.Result);
            }
        }

        private void VailLogin(T_PB_USER user)
        {
            if (user.IsLogic)
            {
                var profile = (App.Current as IApplicationFramework).Profile;
                if (user.UserCode == profile.UserCode && user.PassWord == profile.PassWord)
                {
                    if (!(user.RoleId.Equals("c6d0daae-d383-11e3-9a3b-00241d5227ba") 
                        || user.RoleId.Equals("e069ae6c-d383-11e3-9a3b-00241d5227ba") 
                        || (user.TimeOut != null && user.TimeOut.Value >= DateTime.Now)))
                    {
                        AutoLogout(user);
                    }
                }
                else
                {
                    AutoLogout(user);
                }
            }
            else
            {
                AutoLogout(user);
            }
        }

        #endregion

        #region 初始化菜单和对菜单的点击
        void InitMenu()
        {
            var soure = (App.Current as App).Profile.MenuList.ToList();
            if (soure != null)
            {
                ShowNavigation(new T_BASE_MENU { MenuPath = "PD.Manager.FramwWork.PDNavigationPage" }); 
            }
        }

        void Return_Click(object sender, RoutedEventArgs e)
        {
            ShowNavigation(new T_BASE_MENU { MenuPath = "PD.Manager.FramwWork.PDNavigationPage" });
        }

        void SystemMenuClick(object sender, MenuEventArgs e)
        {
            ShowModule((e.DataContext as T_BASE_MENU));
        }

        void ShowModule(T_BASE_MENU menu)
        {
            if (CurrentModulePage != null && CurrentModulePage.GetType().FullName.Equals(menu.MenuPath))
            {
                return;
            }
            try
            {
                object module = null;
                try
                {
                    module = this.GetType().Assembly.CreateInstance(menu.MenuPath);
                    //if (module == null)
                    //    module = typeof(PD.Display.App).Assembly.CreateInstance(menu.MenuPath);
                }
                catch 
                {

                }
                if (module is BaseModulePage)
                {
                    if (CurrentModulePage is IModule)
                    {
                        try
                        {
                            (CurrentModulePage as IDisposable).Dispose();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    (module as BaseModulePage).Menu = menu;
                    CurrentModulePage = module as BaseModulePage;
                }
            }
            catch
            {
                MessageBox.Show("打开业务功能失败");
            }
            if (CurrentModulePage != null && CurrentModulePage != businessContent.Content)
            {
                businessContent.Content = CurrentModulePage;
                returnPanel.Visibility = System.Windows.Visibility.Visible;
                tbMenu.Text = string.IsNullOrEmpty(menu.MenuName) ? "" : menu.MenuName;
            }
        }

      public  void ShowNavigation(T_BASE_MENU menu)
        {
            ShowModule(menu);
            returnPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion

        #region 对App提供的接口
        public void OpenModuleInMenuCode(string code)
        {
            var menu = (App.Current as IApplicationFramework).Profile.MenuList.FirstOrDefault(item => item.MenuPath.Equals(code));
            if (menu != null)
                ShowModule(menu);
        }

        /// <summary>
        ///   框架开始等待
        /// </summary>
        public void BeginLoading()
        {
            busyIndicator.IsBusy = true;
        }

        /// <summary>
        ///   框架结束等待
        /// </summary>
        public void EndLoading()
        {
            busyIndicator.IsBusy = false;
        }

        public void HiddenCurrentHtmlView()
        {
            if (CurrentModulePage is IReport)
            {
                List<ReportView> view = new List<ReportView>();
                CommonMethod.GetElementCollection<ReportView>(CurrentModulePage, view);
                view.ForEach(page => page.HiddenHtmlView());
            }
        }

        public void ShowCurrentHtmlView()
        {
            if (CurrentModulePage is IReport)
            {
                List<ReportView> view = new List<ReportView>();
                CommonMethod.GetElementCollection<ReportView>(CurrentModulePage, view);
                view.ForEach(page => page.ShowHtmlView());
            }
        }
        #endregion

        void btnInstall_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                MessageBox.Show("您已经在桌面程序中运行!");
                return;
            }
            if (Application.Current.InstallState == InstallState.NotInstalled)
            {
                Application.Current.Install();
            }
            else
            {
                MessageBox.Show("您已经安装了该应用程序");
            }
        }

        void LogoutClick(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        void panelUser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logout();
        }

        void Logout()
        {
            if (MessageBox.Show("是否注销当前用户?", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (LogoutSystem != null)
                    LogoutSystem(this, EventArgs.Empty); 
            }
        }

        void AutoLogout(T_PB_USER user)
        {
            var seializa = new SerializationMemoryObject<T_PB_USER>();
            seializa.Serializer(user);
            IsolatedStorageHelper.WriteSlTextFile("Login", seializa.SerializerData);

            MessageBox.Show("您的用户信息已经失效,已经被禁止使用该系统");
            if (LogoutSystem != null)
                LogoutSystem(this, EventArgs.Empty);

        }

        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="element"></param>
        public void PopupMessage(UIElement element, int width, int height, Thickness th)
        {
        }

        internal void PopupTitle(Uri imageUrl, string title)
        {
        }

        internal void UpdateMessageCount()
        {
        }

        public void RegisterEvent()
        {
            client.UserLoginCompleted += (UserLoginCompleted);
            btnInstall.MouseLeftButtonUp += (btnInstall_MouseLeftButtonUp);
            timer.Tick += (UpdateInTick);
            btnLogout.Click += (LogoutClick); 
            btnReturn.MouseLeftButtonUp += (Return_Click);
            panelUser.MouseLeftButtonDown += (panelUser_MouseLeftButtonDown);
        }

        public void UnRegisterEvent()
        {
            client.UserLoginCompleted -= (UserLoginCompleted);
            btnInstall.MouseLeftButtonUp -= (btnInstall_MouseLeftButtonUp);
            timer.Tick -= (UpdateInTick);
            timer.Stop(); 
            btnReturn.MouseLeftButtonUp -= (Return_Click);
            panelUser.MouseLeftButtonDown -= (panelUser_MouseLeftButtonDown);
            btnLogout.Click -= (LogoutClick);
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }

        bool isMax = false;

        internal void MaxWindow()
        {
            if (isMax)
            {
                DefaultWindow();
            }
            else
            {
                isMax = true;
                panelMain.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
                panelMain.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
                //panelMain.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        internal void DefaultWindow()
        {
            isMax = false; 
            panelMain.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            panelMain.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            //panelMain.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
