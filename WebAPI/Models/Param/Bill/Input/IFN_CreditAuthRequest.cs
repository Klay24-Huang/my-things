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
        /// 付費項目(0 租金,1 罰金(沒在用),2 eTag(沒在用),3 補繳,4 訂閱,5 訂閱,6 春節訂金,7 錢包)
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
        /// 授權目的(1、預約,2、訂金,4、延長用車,3、取車,5、逾時,6、欠費,7、還車)
        /// </summary>
        public int AuthType { get; set; }
    }
}