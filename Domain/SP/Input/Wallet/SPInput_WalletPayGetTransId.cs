using System;

namespace Domain.SP.Input.Wallet
{
    public  class SPInput_WalletPayGetTransId: SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 身分證
        /// </summary>
        public string MemberID { set; get; }
        /// <summary>
        /// 錢包帳號
        /// </summary>
        public string WalletAccountID { set; get; }
        /// <summary>
        /// 扣款金額
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 交易編號前置
        /// </summary>
        public string TransIdLeft { get; set; }
        /// <summary>
        /// 付費項目(0 租金,1 罰金(沒在用),2 eTag(沒在用),3 補繳,4 訂閱,5 訂閱,6 春節訂金,7 錢包,8 主動取款,99 訂閱制)
        /// </summary>
        public int CreditType { set; get; }
        public string PrgName { get; set; }
        public string PrgUser { get; set; }

    }
}
