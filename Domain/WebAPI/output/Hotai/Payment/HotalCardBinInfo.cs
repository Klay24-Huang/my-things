using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class HotalCardBinInfo
    {
        /// <summary>
        /// 銀行代碼
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 發卡銀行描述
        /// </summary>
        public string BankDesc { get; set; }
        /// <summary>
        /// 發卡機構
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// BIN碼
        /// </summary>
        public string BIN { get; set; }
        /// <summary>
        /// 是否偽金融卡(Y：金融卡)
        /// </summary>
        public string IsDebit { get; set; }
        /// <summary>
        /// TokenFlag
        /// </summary>
        public string TokenFlag { get; set; }
        /// <summary>
        /// 上線日期
        /// </summary>
        public DateTime OnlineDate { get; set; }



    }
}
