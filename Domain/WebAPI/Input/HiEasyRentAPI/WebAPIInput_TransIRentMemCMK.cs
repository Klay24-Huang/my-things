namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_TransIRentMemCMK
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { get; set; }

        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 同意書版本類型
        /// </summary>
        public string VERTYPE { get; set; }

        /// <summary>
        /// 同意書版本號
        /// </summary>
        public string VER { get; set; }

        /// <summary>
        /// 同意來源管道
        /// <para>I:IRENT</para>
        /// <para>W:官網</para>
        /// </summary>
        public string VERSOURCE { get; set; }

        /// <summary>
        /// 電話通知狀態
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string TEL { get; set; }

        /// <summary>
        /// 簡訊通知狀態
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string SMS { get; set; }

        /// <summary>
        /// EMAIL通知
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string EMAIL { get; set; }

        /// <summary>
        /// 郵寄通知
        /// <para>N:不通知</para>
        /// <para>Y:通知</para>
        /// </summary>
        public string POST { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string COMPID { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string COMPNM { get; set; }

        /// <summary>
        /// 程式來源
        /// </summary>
        public string PRGID { get; set; }

        /// <summary>
        /// 異動來源
        /// </summary>
        public string USERID { get; set; }
    }
}