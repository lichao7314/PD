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
using System.Diagnostics;
using PD.ServiceClient;
using System.Net.Browser;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using PD.Manager.FramwWork;

namespace PD.Manager
{
    public partial class App : Application, IApplicationFramework
    {
        public App()
        {
            WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);

            if (!IsolatedStorageFile.IsEnabled)
            {
                MessageBox.Show("请启用Silverlight应用程序存储功能。");
                return;
            }

            //检测当前的状态
            //if (!NetworkInterface.GetIsNetworkAvailable())
            //   MessageBox.Show("系统无法联机，请检查网络连接是否正常！", "警告", MessageBoxButton.OK);


            SkinHelper.AddResource(SkinHelper.SkinDictionary.ColorDictionary[SkinHelper.ReadCurrentSkin().SkinType]);

            SkinHelper.AddResource(SkinHelper.SkinDictionary.SkinFontFamilyDictionary[SkinHelper.ReadCurrentSkin().SkinFontFamily]);

            SkinHelper.AddResource(SkinHelper.SkinDictionary.SkinFontSizeDictionary[SkinHelper.ReadCurrentSkin().SkinFontSize]);

            // var sd = new DisplayDesign();
            //var  Source = DesignDataUpdate.GetHouseInStorage();
            //  sd.SetDesign(Source.FirstOrDefault(item=>item.designCode.Equals("design1")));

            RootVisual = new PDMainPage();
            CheckVersionUpdate();
            
            GeneralConfigs.Initialize("ConstantConfiguration.txt");
            GeneralConfigs.InitializeCompleted += (ConstantInitializeCompleted);
            this.UnhandledException += Application_UnhandledException;
        }

    

        void ConstantInitializeCompleted()
        {
            try
            {
                //SystemConstantConfig.ReportAddress = GeneralConfigs.SeachConfigValue("reportAddress");
                SystemConstantConfig.EnableOperation = Convert.ToBoolean(GeneralConfigs.SeachConfigValue("EnableOperation"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取SystemConstantConfig文件出错," + ex.Message);
            }
        }

        #region OOB模式

        private static void CheckVersionUpdate()
        {
            if (Current.InstallState == InstallState.Installed)
            {
                Current.CheckAndDownloadUpdateCompleted += CurrentCheckAndDownloadUpdateCompleted;
                Current.CheckAndDownloadUpdateAsync();
            }
        }

        private static void CurrentCheckAndDownloadUpdateCompleted(object sender,
                                                                   CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MessageBox.Show("已成功下载应用程序的新版本并将启动现有应用程序，重新启动时使用此新版本。", "系统提示", MessageBoxButton.OK);
                Current.MainWindow.Close();
            }
            if (e.Error != null)
            {
                var msg = "下载应用程序的新版本失败！请确定网络连接正常，或者在浏览器内打开应用程序。";
                msg += "\r\n详细错误信息:" + e.Error.Message;
                MessageBox.Show(msg, "警告", MessageBoxButton.OK);
            }
        }

        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        #region 应用程序异常处理
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            #region 处理未捕获异常

            try
            {
                if (ExceptionHelper.GetInnerException(e.ExceptionObject).Message.IndexOf("COM 组件") >= 0 ||
                    ExceptionHelper.GetInnerException(e.ExceptionObject).Message.IndexOf("COMException") >= 0 ||
                    ExceptionHelper.GetInnerException(e.ExceptionObject).Message.IndexOf("HRESULT0x8000FFFF (E_UNEXPECTED)") >= 0)
                {
                    //目前不知道什么原因激发的COM错误，暂时屏蔽掉COM错误
                }
                else
                {
                    this.HiddenHtmlView();
                    ExceptionHelper.ShowExceptionWindow(e.ExceptionObject, AppExceptionWindowClosed);
                }
            }
            catch
            {
            }
            e.Handled = true;

            #endregion

            // 如果应用程序是在调试器外运行的，则使用浏览器的
            // 异常机制报告该异常。在 IE 上，将在状态栏中用一个 
            // 黄色警报图标来显示该异常，而 Firefox 则会显示一个脚本错误。
            if (!Debugger.IsAttached)
            {
                // 注意: 这使应用程序可以在已引发异常但尚未处理该异常的情况下
                // 继续运行。 
                // 对于生产应用程序，此错误处理应替换为向网站报告错误
                // 并停止应用程序。
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ReportErrorToDOM(e);
                });
            }
        }

        public void AppExceptionWindowClosed(object sender, EventArgs e)
        {
            this.ShowHtmlView();
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                //string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                //errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");
                //System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region IApplicationFramework
        public void ShowModule(string menuCode)
        {
            PDFrameWork.CurrentFrameWork.OpenModuleInMenuCode(menuCode);
        }

        public void BeginLoading()
        {
            PDFrameWork.CurrentFrameWork.BeginLoading();
        }

        public void EndLoading()
        {
            PDFrameWork.CurrentFrameWork.EndLoading();
        }

        public void BeginWindowLoading(DependencyObject element)
        {
            PDFrameWork.CurrentFrameWork.CurrentModulePage.BeginWindowLoading(element);
        }

        public void EndWindowLoading(DependencyObject element)
        {
            PDFrameWork.CurrentFrameWork.CurrentModulePage.EndWindowLoading(element);
        }

        public string IPAddress
        {
            get
            {
                return "..";
            }
        }

        public void HiddenHtmlView()
        {
            PDFrameWork.CurrentFrameWork.HiddenCurrentHtmlView();
        }

        public void ShowHtmlView()
        {
            PDFrameWork.CurrentFrameWork.ShowCurrentHtmlView();
        }
        #endregion

        public ServiceClient.PDService.T_PB_USER Profile
        {
            get;
            set;
        }

        public DateTime ServerTime
        {
            get
            {
                if (ServerTimeServer.TimeServer == null)
                    return DateTime.Now;
                return ServerTimeServer.TimeServer.CurrentServerTime;
            }
        }

        public event EventHandler ClosePopup;

        public void ExcuteClosePopup()
        {
            if (ClosePopup != null)
                ClosePopup(this, EventArgs.Empty);
        }

        public void UpdateMessageCount()
        {
            PDFrameWork.CurrentFrameWork.UpdateMessageCount();
        }

        public void MaxWindow()
        {
            PDFrameWork.CurrentFrameWork.MaxWindow();
        }

        public void DefaultWindow()
        {
            PDFrameWork.CurrentFrameWork.DefaultWindow();

        }

        public event EventHandler UpdateDataCompleted;

        public void ExcuteUpdate()
        {
            if (UpdateDataCompleted != null)
            {
                UpdateDataCompleted(this, EventArgs.Empty);
            }
        }

        public void ShowNavigation(ServiceClient.PDService.T_BASE_MENU menu)
        {
            PDFrameWork.CurrentFrameWork.ShowNavigation(menu);
        }
    }
}
