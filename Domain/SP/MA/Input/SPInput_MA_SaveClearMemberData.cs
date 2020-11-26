using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.MA.Input
{
   public class SPInput_MA_SaveClearMemberData:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 據點
        /// </summary>
        public string Station { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Lng { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Lat { set; get; }
    }
}
