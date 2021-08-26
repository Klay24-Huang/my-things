namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletTransferCheck
    {
        /// <summary>
        /// 驗證結果(1成功,0失敗)
        /// </summary>
        public int CkResult { get; set; } = 0;
        /// <summary>
        /// 名稱或電話號碼
        /// </summary>
        public string Name_Phone { get; set; }
        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { get; set; }
    }
}