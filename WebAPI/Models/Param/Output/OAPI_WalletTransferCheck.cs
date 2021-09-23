namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletTransferCheck
    {
        /// <summary>
        /// 驗證結果(1成功,0失敗)
        /// </summary>
        public int CkResult { get; set; } = 0;
        /// <summary>
        /// 受贈者名稱
        /// </summary>
        public string ShowName { get; set; }
        /// <summary>
        /// 顯示ID或電話號碼
        /// </summary>
        public string ShowValue { get; set; }
        /// <summary>
        /// 受贈人ID
        /// </summary>
        public int IDNO { get; set; }
    }
}