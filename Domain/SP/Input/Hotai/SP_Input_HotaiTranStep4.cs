using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Hotai
{
    public class SP_Input_HotaiTranStep4: SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
        /// <summary>
        /// 特店交易編號
        /// </summary>
        public string MerchantTradeNo { get; set; } = "";
        /// <summary>
        /// 和泰會員OneID
        /// </summary>
        public string MemberID { get; set; } = "";
        /// <summary>
        /// MerchantID 
        /// </summary>
        public string MerchantID { get; set; } = "";
        /// <summary>
        /// 特店編號
        /// </summary>
        public int MerID { get; set; }
        /// <summary>
        /// 和泰回覆碼
        /// </summary>
        public string RequestNo { get; set; } = "";
        /// <summary>
        /// 授權回傳錯誤代碼
        /// </summary>
        public string AuthErrcode { get; set; } = "";
        /// <summary>
        /// 授權結果
        /// </summary>
        public int AuthStatus { get; set; }
        /// <summary>
        /// 授權結果代碼
        /// </summary>
        public string AuthStatusCode { get; set; } = "";
        /// <summary>
        /// 授權狀態描述
        /// </summary>
        public string AuthStatusDesc { get; set; } = "";
        /// <summary>
        /// 授權同意金額
        /// </summary>
        public int AuthAmt { get; set; }
        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; } = "";
        /// <summary>
        /// SSL授權交易代碼
        /// </summary>
        public string Authrrpid { get; set; } = "";
        /// <summary>
        /// 是否分期
        /// </summary>
        public int NumberOPay { get; set; }
        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; } = "";
        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }
        /// <summary>
        /// 中信交易編號
        /// </summary>
        public string Xid { get; set; } = "";
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNumber { get; set; } = "";
        /// <summary>
        /// 卡號末四碼
        /// </summary>
        public string Last4digitPAN { get; set; } = "";
        /// <summary>
        /// 請款日期
        /// </summary>
        public string MerchantMemberID { get; set; } = "";
        /// <summary>
        /// 回復碼(TB_Trade)
        /// </summary>
        public string RetCode { get; set; } = "";
        /// <summary>
        /// 回復訊息(TB_Trade)
        /// </summary>
        public string RetMsg { get; set; } = "";
        /// <summary>
        /// 處理日期(TB_Trade)
        /// </summary>
        public DateTime process_date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AuthIdResp { get; set; }
        /// <summary>
        /// 是否成功 0:未送出;1:成功;-1:失敗;-2:連線失敗
        /// </summary>
        public int IsSuccess { get; set; }
        /// <summary>
        /// 可否關帳
        /// </summary>
        public int ChkClose { get; set; }
        /// <summary>
        /// 授權單位(0)
        /// </summary>
        public int CardType { get; set; }
        /// <summary>
        /// 預授權授權類別
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 修改程式代號
        /// </summary>
        public string ProName { get; set; } = "";
        /// <summary>
        /// 修改者
        /// </summary>
        public string UserID { get; set; } = "";

        /// <summary>
        /// PretStep
        /// </summary>
        public int PretStep { get; set; }

    }
}
