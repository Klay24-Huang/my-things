using System;

namespace Domain.SP.Input.Wallet
{
    public class SPInput_GetWalletStoreTradeTransHistory : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 查詢起日
        /// </summary>
        public DateTime SD { get; set; }

        /// <summary>
        /// 查詢迄日
        /// </summary>
        public DateTime ED { get; set; }
    }
}