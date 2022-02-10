using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_GetCTBCInquiryList: SP_Input_CTBCCapBase
    {
        /// <summary>
        /// 請款查詢起日
        /// </summary>
        public string QueryBgn { get; set; }

        /// <summary>
        /// 請款查詢迄日
        /// </summary>
        public string QueryEnd { get; set; }
    }
}
