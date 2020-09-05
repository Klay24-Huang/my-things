using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
   public  class SPInput_ChangePWD
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }

        ///// 機碼
        ///// </summary>
        //public string DeviceID { set; get; }
        ///// <summary>
        /// 舊密碼
        /// </summary>
        public string OldPWD { set; get; }
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPWD { set; get; }
        /// <summary>
        /// 此筆呼叫的log id
        /// </summary>
        public Int64 LogID { set; get; }
  
    }
}
