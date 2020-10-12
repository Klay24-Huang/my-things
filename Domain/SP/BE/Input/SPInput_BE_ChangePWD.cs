using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ChangePWD:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }

        ///// 機碼
        ///// </summary>
        //public string DeviceID { set; get; }
        ///// <summary>
        /// 舊密碼
        /// </summary>
        public string UserPwd { set; get; }
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPwd { set; get; }
    }
}
