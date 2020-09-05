using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Register
{
    public class SPOutput_ForgetPWD:SPOutput_Base
    {
        /// <summary>
        /// 此會員註冊的手機
        /// </summary>
        public string Mobile { set; get; }
    }
}
