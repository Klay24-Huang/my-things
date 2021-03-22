using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_UpdCATDeviceToken : SPInput_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 遠傳車機ID
        /// </summary>
        public string deviceId { set; get; }
        /// <summary>
        /// 遠傳車機Token
        /// </summary>
        public string deviceToken { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
