﻿using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ContactFinish:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        public string UserID { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// 強還時間
        /// </summary>
        public DateTime ReturnDate { set; get; }

    }
}
