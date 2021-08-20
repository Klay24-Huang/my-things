using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletInfoCheck
    {
        /// <summary>
        /// 身分證
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string PhoneNo { get; set; }
    }
}