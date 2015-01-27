using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Reflection;

namespace PD.ServiceClient
{
    public static class ServiceHelper
    {
        public static TServiceClient CreateWcfServiceClient<TServiceClient, TService>(this string t)
            where TServiceClient : System.ServiceModel.ClientBase<TService>, TService
            where TService : class
        {
            return WcfServiceClientFactory<TServiceClient, TService>.CreateServiceClient(t);
        }

        public static class WcfServiceClientFactory<TServiceClient, TService>
            where TServiceClient : System.ServiceModel.ClientBase<TService>, TService
            where TService : class
        {
            public static TServiceClient CreateServiceClient(string serviceAddress)
            {
                var url = new Uri(Application.Current.Host.Source, serviceAddress);
                var endpointAddr = new EndpointAddress(url);
                var binding = new BasicHttpBinding();
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.ReceiveTimeout = TimeSpan.Parse("00:10:00");
                binding.SendTimeout = TimeSpan.Parse("00:10:00");
                binding.OpenTimeout = TimeSpan.Parse("00:10:00");
                binding.CloseTimeout = TimeSpan.Parse("00:10:00");
               
                ConstructorInfo ctor =
                    typeof(TServiceClient).GetConstructor(new[] { typeof(System.ServiceModel.Channels.Binding), typeof(EndpointAddress) });
                return (TServiceClient)ctor.Invoke(new object[] { binding, endpointAddr });
            }

        }
    }
}
