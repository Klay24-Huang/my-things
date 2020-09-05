using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 重發email
    /// </summary>
    public class IAPI_ReSendEMail:IAPI_Base
    {

        /// <summary>
        /// 會員email
        /// </summary>
        public string MEMEMAIL { set; get; }
    }
}