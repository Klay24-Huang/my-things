using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Register
{
    public class SPOutput_GetVerifyCode : SPOutput_Base
    {
        /// <summary>
        /// 簡訊驗證碼
        /// </summary>
        public string VerifyCode { get; set; }
    }
}