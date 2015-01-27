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
using PD.ServiceClient.PDService;

namespace PD.ServiceClient
{
    public class ClientProxy
    {
        #region Wcf client instance;
        /// <summary>
        /// 装修展示客户端
        /// </summary>
        private PDServiceClient _client;
        /// <summary>
        ///   Gets the basicData client.
        /// </summary>
        public PDServiceClient Client
        {
            get
            {
                return _client ??
                      (_client =
                       WcfConfigs.Instance.GetConfig("PDService").Uri.CreateWcfServiceClient
                           <PDServiceClient, PD.ServiceClient.PDService.PDService>());
            }
        }
        #endregion
    }
}
