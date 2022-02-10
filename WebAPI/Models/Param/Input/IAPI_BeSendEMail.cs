using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.BackEnd.Input;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 後台專用發email
    /// </summary>
    public class IAPI_BeSendEMail : IAPI_BE_Base
    {
        public string IDNO { set; get; }
        /// <summary>
        /// 會員email
        /// </summary>
        public string MEMEMAIL { set; get; }
        public string TITLE { set; get; }
        public string CONTENT { set; get; }
    }
}