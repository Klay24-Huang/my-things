using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Bill.Input
{
    public class IFN_CreditAuthRequest
    {   
        /// <summary>
        /// 指定付費方式(0:信用卡 1:錢包 4:和泰Pay)
        /// </summary>
        public int CheckoutMode { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 付費金額
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 付費項目(0 租金,1 罰金(沒在用),2 eTag(沒在用),3 補繳,4 訂閱,5 訂閱,6 春節訂金,7 錢包,8 主動取款,99 訂閱制)
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 自動關帳(0:不關帳 1:自動關帳)
        /// </summary>
        public int autoClose { get; set; }
        /// <summary>
        /// 來源程式
        /// </summary>
        public string funName { get; set; }
        /// <summary>
        /// 執行程式人員
        /// </summary>
        public string insUser { get; set; }
        /// <summary>
        /// 授權目的(1、預約,2、訂金,4、延長用車,3、取車,5、逾時,6、欠費,7、還車,8、訂閱制,9、錢包儲值,10、主動取款)
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 錢包用交易類型(tbWalletTradeMain)
        /// </summary>
        public string TradeType { get; set; } = "";

        public Int64 LogID { get; set; } = 0;
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; } = "";
        /// <summary>
        /// 輸入來源(1:APP;2:Web)
        /// 1:驗證Token/2:不驗證Token
        /// </summary>
        public short InputSource { get; set; } = 2;
        /// <summary>
        /// 專案類型(0:同站;3:路邊;4:機車)
        /// </summary>
        public int ProjType { get; set; } = -1;

    }
}