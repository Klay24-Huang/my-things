using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Battle
{
    public class SPInput_GetKey:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string VerID { set; get; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string VerPwd { set; get; }
    }
}
