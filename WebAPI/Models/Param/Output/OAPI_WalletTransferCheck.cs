namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletTransferCheck
    {
        /// <summary>
        /// 驗證結果(1成功,0失敗)
        /// </summary>
        public int CkResult { get; set; } = 0;
        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        //public int WalletAmount { get; set; } = 0;
        /// <summary>
        /// 當月入賬總金額
        /// </summary>
        //public int MonTransIn { get; set; } = 0;

        #region mark

        ///// <summary>
        ///// 姓名
        ///// </summary>
        //public string Name { get; set; }

        ///// <summary>
        ///// 電話
        ///// </summary>
        //public string PhoneNo { get; set; }

        ///// <summary>
        ///// 轉贈金額
        ///// </summary>
        //public int Amount { get; set; }

        #endregion
    }
}