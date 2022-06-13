using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsHotaipayEdmLog_I01
    {
        /// <summary>
        /// 會員編號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 寄送EMAIL
        /// </summary>
        public string EMAIL { get; set; }
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string USERID { get; set; }
        /// <summary>
        /// LogID
        /// </summary>
        public Int64 LogID { get; set; }
    }
}
