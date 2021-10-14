using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_DonePayBack: SPInput_Base
    {
        /// <summary>
        /// 欠款查詢主表ID
        /// </summary>
        public int NPR330Save_ID { get; set; }
        public string IDNO { get; set; }
        public string MerchantTradeNo { get; set; }     //20210106 ADD BY ADAM REASON.增加關聯存檔
        public string TaishinTradeNo { get; set; }      //20210106 ADD BY ADAM REASON.增加關聯存檔
        public string Token { get; set; }
        /// <summary>
        /// 付費方式：0:信用卡;1:和雲錢包;2:line pay;3:街口支付
        /// </summary>
        public int PayMode { get; set; }
    }
}
