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
using PD.ServiceClient;

namespace PD.Controls
{
    public class ServerTimeServer  
    {
        public static ServerTimeServer  TimeServer { get; set; }

        private long loginStartTick = 0;

        private DateTime serverTime=DateTime.Now;

        private TimeSpan timeSpan;

        PDServiceClient client = new ClientProxy().Client;

        public DateTime CurrentServerTime
        {
            get
            {
                var currentTime = (Environment.TickCount - loginStartTick);
                return serverTime + TimeSpan.FromMilliseconds(currentTime);
            }
        }

        public ServerTimeServer()
        {
            loginStartTick = Environment.TickCount;
        }

        public void GetTime()
        {
            TimeServer = this;
            client.GetServerTimeCompleted +=  (client_GetServerTimeCompleted);
            client.GetServerTimeAsync();
        }

        void client_GetServerTimeCompleted(object sender, GetServerTimeCompletedEventArgs e)
        {
            client.GetServerTimeCompleted -= (client_GetServerTimeCompleted);
            loginStartTick = Environment.TickCount;
            serverTime = e.Result;
        }
    }
}
