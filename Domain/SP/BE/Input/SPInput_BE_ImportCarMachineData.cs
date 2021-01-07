using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ImportCarMachineData:SPInput_Base
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID       {set;get;}
        /// <summary>
        /// 門號
        /// </summary>
        public string MobileNum {set;get;}
        /// <summary>
        /// 卡號
        /// </summary>
        public string SIMCardNo {set;get;}
        /// <summary>
        /// 遠傳車機編號
        /// </summary>
        public string deviceToken { set; get; }
        //20210105Eric加存放地點
        /// <summary>
        /// 存放地點
        /// </summary>
        public string depositary { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
