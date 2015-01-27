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

namespace PD.Manager.FramwWork
{
    public partial class PDNavigationPage : BaseModulePage, IModule
    {
        public PDNavigationPage()
        {
            InitializeComponent();
            RegisterEvent();
        }

        public void RegisterEvent()
        {
            Loaded += (CRMNavigationPage_Loaded);
        }

        void CRMNavigationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var soure = (App.Current as App).Profile.MenuList.ToList();
            soure.ForEach(item =>
            {
                PDNavigationItem navigationItem = new PDNavigationItem();
                navigationItem.NavigatonClick += (navigationItem_NavigatonClick);
                navigationItem.InitNavigationItem(item);
                //基础数据
                if (item.MenuID.Equals("23d93816-d394-11e3-9a3b-00241d5227ba") ||
                    item.MenuID.Equals("73b0132b-d393-11e3-9a3b-00241d5227ba") ||
                    item.MenuID.Equals("23d93816-d394-11e3-9a3b-00241d5227ba") ||
                    item.MenuID.Equals("73b0132b-d393-11e3-9a3b-00241d5227ba") ||
                    item.MenuID.Equals("95fcfc43-db65-11e3-9408-00241d5227ba") ||
                    item.MenuID.Equals("1da37fc2-dbd3-11e3-ba3d-00e0662aeec5") ||
                    item.MenuID.Equals("1da37fc2-dbd3-11e3-ba3d-00e0662aeec5"))
                {
                    //imCHXX.Visibility = System.Windows.Visibility.Visible;
                    //tbCHXX.Visibility = System.Windows.Visibility.Visible;
                    panelCHXX.Visibility = System.Windows.Visibility.Visible;
                    panelCHXX.Children.Add(navigationItem);
                }
                //设计和产品
                if (item.MenuID.Equals("73b0132b-d393-11e3-9a3b-01241d5227ba") || item.MenuID.Equals("9f4a03c5-dcd9-11e3-a817-00e0662a1221") || item.MenuID.Equals("9f4a03c5-dcd9-11e3-a817-00e0662aeec5"))
                {
                    //imCWGL.Visibility = System.Windows.Visibility.Visible;
                    //tbCWGL.Visibility = System.Windows.Visibility.Visible;
                    panelCWGL.Visibility = System.Windows.Visibility.Visible;
                    panelCWGL.Children.Add(navigationItem);
                }
                //产品展示
                if (item.MenuID.Equals("9f4a03c5-dcd9-11e3-a817-00e0662a1222") || item.MenuID.Equals("83b0132b-d393-11e3-9a3b-01241d5227ba") || item.MenuID.Equals("8c4fbe5d-e243-11e3-888f-00e0662aeec5"))
                {
                    panelYWGL.Children.Add(navigationItem);
                    panelYWGL.Visibility = System.Windows.Visibility.Visible;
                    //imYWGL.Visibility = System.Windows.Visibility.Visible;
                    //tbYWGL.Visibility = System.Windows.Visibility.Visible;
                }

            });
        }

        void navigationItem_NavigatonClick(object sender, MenuNavigationEventArgs e)
        {
            (App.Current as IApplicationFramework).ShowModule(e.Menu.MenuPath);
        }

        public void UnRegisterEvent()
        {
            Loaded -= (CRMNavigationPage_Loaded);
            DisposePanel(panelJCSJ);
            DisposePanel(panelYWGL);
            DisposePanel(panelCWGL);
            DisposePanel(panelCHXX);
        }

        private void DisposePanel(Panel panel)
        {
            panel.Children.ToList().ForEach(child =>
            {
                if (child is PDNavigationItem)
                {
                    (child as PDNavigationItem).Dispose();
                }
            });
        }

        public void Dispose()
        {
            UnRegisterEvent();
        }
    }
}
