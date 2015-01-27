using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using System.Data;
using System.IO;


namespace PD.Services
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class PDService
    {
        /// <summary>
        /// 系统时间
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }
    }
}
