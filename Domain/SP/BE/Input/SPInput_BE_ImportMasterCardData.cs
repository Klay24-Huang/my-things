using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ImportMasterCardData : SPInput_Base
    {
        /// <summary>
        /// 員編
        /// </summary>
        public string ManagerId { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        public string UserID { set; get; }
    }
}
