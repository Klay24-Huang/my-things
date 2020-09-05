using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    public class SPInput_CheckAccount
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 此筆呼叫的id
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
