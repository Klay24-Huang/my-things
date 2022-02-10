namespace Domain.SP.Input.Wallet
{
    public class SPInput_WalletStoredErrorHandle
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Seqno { get; set; }

        /// <summary>
        /// 處理狀態(0:未處理;1:處理成功;2:處理失敗)
        /// </summary>
        public int ProcessStatus { get; set; }

        /// <summary>
        /// 回傳代碼
        /// </summary>
        public string ReturnCode { get; set; }

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 回傳異常錯誤訊息
        /// </summary>
        public string ExceptionData { get; set; }

        /// <summary>
        /// 程式名稱
        /// </summary>
        public string PRGName { get; set; }

    }
}