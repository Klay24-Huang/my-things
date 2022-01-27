using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandleCarSetting:SPInput_Base
    {
        /// <summary>
        /// 隸屬據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 目前據點代碼
        /// </summary>
        public string nowStationID { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 操作者員編
        /// </summary>
        public string UserID { set; get; }
    }
}
