using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletInfoCheck
    {
        /// <summary>
        /// 身分證或手機號碼
        /// </summary>
        public string IDNO_Phone { get; set; }
    }
}