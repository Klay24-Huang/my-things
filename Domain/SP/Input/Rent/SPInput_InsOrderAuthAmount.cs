using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_InsOrderAuthAmount : SPInput_Base
    {

        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 廠商訂單編號
        /// </summary>
        public string MerchantTradNo { get; set; }

        /// <summary>
        /// 銀行交易序號
        /// </summary>
        public string BankTradeNo { get; set; }

        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 信用卡類別(0:和泰 ; 1:台新)
        /// </summary>
        public int CardType { get; set; }

        /// <summary>
        /// 授權類別 (1:預約; 2:訂金; 3:取車; 4:延長用車; 5:逾時; 6:欠費; 7:還車)
        /// </summary>
        public int AuthType { get; set; }

        /// <summary>
        /// 授權金額
        /// </summary>
        public int final_price { get; set; }

        /// <summary>
        /// 處理狀態 (0:未處理; 1:處理中; 2:已處理)
        /// </summary>
        public int Status { get; set; }

    }
}
