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
using PD.ServiceClient;

namespace PD.Manager
{
    public partial class MainPage : UserControl
    {
        PDServiceClient client = null;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += (MainPage_Loaded);
        }



        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            client = new ClientProxy().Client;
            this.btnQuery.Click += (btnQuery_Click);
            client.AddAddressCompleted += (client_AddAddressCompleted);
            client.GetMenuCompleted += (client_GetMenuCompleted);
        }

        void client_GetMenuCompleted(object sender, GetMenuCompletedEventArgs e)
        {

        }

        void client_AddAddressCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            txtPath.Text = string.Empty;
        }

        void btnQuery_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtPath.Text.Trim()))
                client.AddAddressAsync(txtPath.Text.Trim());
        }
    }
}
