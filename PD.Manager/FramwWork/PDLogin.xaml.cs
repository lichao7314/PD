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
using PD.ServiceClient.PDService;
using System.Net.NetworkInformation;
using PD.Controls.FrameWork;

namespace PD.Manager.FramwWork
{
    public partial class PDLogin : UserControl, ILogin, IModule
    {
        PDServiceClient client =  new ClientProxy().Client;

        public event EventHandler LoginCompleted;

        public PDLogin()
        {
            InitializeComponent();
            RegisterEvent();
        }

        void PDLogin_Loaded(object sender, RoutedEventArgs e)
        {
            GetInfo();
            this.tb_userCode.Focus();
            this.tb_userCode.SelectAll();
        }

        void ReturnLoginClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "login", true);
        }

        void ChangPwClick(object sender, MouseButtonEventArgs e)
        {
            var pw = new PDChangePw();
            pw.Show("修改密码", true);
        }

        #region 登录
        void LoginClick(object sender, MouseButtonEventArgs e)
        {
            var userCode = tb_userCode.Text.Trim();
            var passWord = tb_password.Password.Trim();
            if (string.IsNullOrEmpty(userCode))
            {
                MessageBox.Show("请输入帐号");
                tb_userCode.Focus();
                return;
            }
            if (string.IsNullOrEmpty(passWord))
            {
                MessageBox.Show("请输入密码");
                tb_password.Focus();
                return;
            }

            if (!IsolatedStorageHelper.IncreaseQuotaTo())
                return;

            VisualStateManager.GoToState(this, "logining", true);

            SetInfo();
            T_PB_USER user = new T_PB_USER() { IsLogic=true,IsLogin=1 };
            user.UserName = "测试用户";
            user.PassWord = "测试用户";
            user.MenuList = new System.Collections.ObjectModel.ObservableCollection<T_BASE_MENU>();
            user.MenuList.Add(new T_BASE_MENU { MenuID = "1", MenuName = "功能1", MenuImagePath = "/Images/FrameWork/Navigation/1.png" });
            user.MenuList.Add(new T_BASE_MENU { MenuID = "1", MenuName = "功能2", MenuImagePath = "/Images/FrameWork/Navigation/1.png" });
            user.MenuList.Add(new T_BASE_MENU { MenuID = "1", MenuName = "功能3", MenuImagePath = "/Images/FrameWork/Navigation/1.png" });
            user.MenuList.Add(new T_BASE_MENU { MenuID = "1", MenuName = "功能4", MenuImagePath = "/Images/FrameWork/Navigation/1.png" });
            VailLogin(user);
            return;
            //if (NetworkInterface.GetIsNetworkAvailable())
            //{
            //    client.UserLoginAsync(tb_userCode.Text, tb_password.Password, CurrentSystemType.Instance.SystemType);
            //}
            //else
            //{
            //    var seializa = new SerializationMemoryObject<T_PB_USER>();
            //    seializa.SerializerData = IsolatedStorageHelper.ReadSlTextFile("Login");
            //    if (string.IsNullOrEmpty(seializa.SerializerData))
            //    {
            //        MessageBox.Show("没有网络连接");
            //        VisualStateManager.GoToState(this, "loginerror", true);
            //        return;
            //    }
            //    var user= seializa.Deserialize();

            //     //管理员 和设计人员不限制日期
            //    if (user.UserCode == tb_userCode.Text && user.PassWord == tb_password.Password)
            //    {
            //        if (user.RoleId.Equals("c6d0daae-d383-11e3-9a3b-00241d5227ba") || user.RoleId.Equals("e069ae6c-d383-11e3-9a3b-00241d5227ba") || (user.TimeOut != null && user.TimeOut.Value >= DateTime.Now))
            //        {
            //            VailLogin(user);
            //        }
            //        else
            //        {
            //            MessageBox.Show(user.UserName + "用户已过期,使用期到" + user.TimeOut.Value.ToString("yyyy-MM-dd"));
            //            VisualStateManager.GoToState(this, "loginerror", true);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("用户名密码错误");
            //        VisualStateManager.GoToState(this, "loginerror", true);
            //    }
            //}
        }

        void UserLoginCompleted(object sender, UserLoginCompletedEventArgs e)
        {
            //if (e.Error == null)
            //{
                VailLogin(e.Result);
            //}
            //else
            //{
            //    MessageBox.Show(e.Error.Message);
            //    VisualStateManager.GoToState(this, "loginerror", true);
            //}
        }

        private void VailLogin(T_PB_USER user)
        {
            if (user.IsLogic)
            {
                (App.Current as App).Profile = user;

                var seializa = new SerializationMemoryObject<T_PB_USER>();
                seializa.Serializer(user);
                IsolatedStorageHelper.WriteSlTextFile("Login", seializa.SerializerData);

                if (LoginCompleted != null)
                    LoginCompleted(this, EventArgs.Empty);
            }
            else
                VisualStateManager.GoToState(this, "loginerror", true);
        }
        #endregion

        void PassWordEnter(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.tb_password.Password) && e.Key == Key.Enter)
            {
                LoginClick(null, null);
            }
        }

        void UserCodeEnter(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_userCode.Text) && e.Key == Key.Enter)
                tb_password.Focus();
        }

        #region 获取和设置登录信息到隔离缓存
        void SetInfo()
        {
            IsolatedStorageHelper.WriteSlTextFile("EECrm", tb_userCode.Text + "!@#" + tb_password.Password);
        }

        void GetInfo()
        {
            var info = IsolatedStorageHelper.ReadSlTextFile("EECrm");
            if (!string.IsNullOrEmpty(info))
            {
                string[] infos = info.Split(new string[] { "!@#" }, StringSplitOptions.RemoveEmptyEntries);
                if (infos.Length >= 2)
                {
                    tb_userCode.Text = infos[0];
                    tb_password.Password = infos[1];
                }
            }
        }
        #endregion

        void btnBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PDMainPage.Main.Init();
        }

        void lbUserName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tb_userCode.Focus();
        }

        void lbPassWord_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tb_password.Focus();
        }

        void tb_userCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetWaterState();
        }

        void tb_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetWaterState();
        }

        private void SetWaterState()
        {
            if (string.IsNullOrEmpty(this.tb_userCode.Text))
                this.lbUserName.Visibility = System.Windows.Visibility.Visible;
            else
                this.lbUserName.Visibility = System.Windows.Visibility.Collapsed;

            if (string.IsNullOrEmpty(this.tb_password.Password))
                this.lbPassWord.Visibility = System.Windows.Visibility.Visible;
            else
                this.lbPassWord.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void RegisterEvent()
        {
            client.UserLoginCompleted += (UserLoginCompleted);
            returnbtn.Click += (ReturnLoginClick);
            btnBack.MouseLeftButtonUp +=    (btnBack_MouseLeftButtonUp);
            btnLogin.MouseLeftButtonUp += LoginClick;
            btnChangPW.MouseLeftButtonUp += (ChangPwClick);
            Loaded += (PDLogin_Loaded);
            tb_userCode.KeyUp += (UserCodeEnter);
            tb_password.KeyUp += (PassWordEnter); tb_password.PasswordChanged += (tb_password_PasswordChanged);
            tb_userCode.TextChanged += (tb_userCode_TextChanged);
            lbPassWord.MouseLeftButtonDown += (lbPassWord_MouseLeftButtonDown);
            lbUserName.MouseLeftButtonDown += (lbUserName_MouseLeftButtonDown);
        }

    

        public void UnRegisterEvent()
        {
            client.UserLoginCompleted -= (UserLoginCompleted);
            btnBack.MouseLeftButtonUp -= (btnBack_MouseLeftButtonUp); 
            returnbtn.Click -= (ReturnLoginClick);
            btnLogin.MouseLeftButtonUp -= LoginClick;
            btnChangPW.MouseLeftButtonUp -= (ChangPwClick);
            Loaded -= (PDLogin_Loaded);
            tb_userCode.KeyUp -= (UserCodeEnter);
            tb_password.KeyUp -= (PassWordEnter); 
            tb_password.PasswordChanged -= (tb_password_PasswordChanged);
            tb_userCode.TextChanged -= (tb_userCode_TextChanged);
            lbPassWord.MouseLeftButtonDown -= (lbPassWord_MouseLeftButtonDown);
            lbUserName.MouseLeftButtonDown -= (lbUserName_MouseLeftButtonDown);
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }
    }

    public interface ILogin
    {
        event EventHandler LoginCompleted;
    }
}
