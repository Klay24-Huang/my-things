using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Flow.Hotai
{
    public class IFN_HotaiPaymentAuth:IFN_HotaiPaymenyBase
    {
        public string CardToken { get; set; }

        /// <summary>
        /// 請求付款時的交易單號(IRent產)
        /// </summary>
        public string Transaction_no { get; set; }
        /// <summary>
        /// 付費金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 行銷活動代碼（中信提供）(非必要欄位，請請固定填"")
        /// </summary>
        public string PromoCode { get; set; }

        //以下為IRent需要欄位

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 自動關帳(0:不關帳 1:自動關帳)
        /// </summary>
        public int AutoClose { get; set; }
        /// <summary>
        /// 付費項目(0 租金,1 罰金(沒在用),2 eTag(沒在用),3 補繳,4 訂閱,5 訂閱,6 春節訂金,7 錢包)
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 授權目的(1、預約,2、訂金,4、延長用車,3、取車,5、逾時,6、欠費,7、還車)
        /// </summary>
        public int AuthType { get; set; }

        //public string PaySuff { get; set; }

        public int Step { get; set; }



    }
}
