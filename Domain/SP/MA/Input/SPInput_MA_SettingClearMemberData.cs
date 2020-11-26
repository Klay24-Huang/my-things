using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.MA.Input
{
    public class SPInput_MA_SettingClearMemberData:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 據點，以;做串接
        /// </summary>
        public string Station { set; get; }
    }
}
