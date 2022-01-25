using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Member
{
    public class SPInput_GetMemberData
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }

        /// <summary>
        /// 是否檢查Token (0:不檢查 1:要檢查)
        /// </summary>
        public int CheckToken { set; get; } = 1;
    }
}
