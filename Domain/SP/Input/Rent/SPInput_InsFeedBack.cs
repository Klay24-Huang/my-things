using System;

namespace Domain.SP.Input.Rent
{
    public class SPInput_InsFeedBack : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 來源：0:取車;1:還車;2:關於iRent
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 回饋類別
        /// </summary>
        public string FeedBackKind { get; set; }

        /// <summary>
        /// 回饋內容
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 星星數，當mode=1時才有意義
        /// </summary>
        public int Star { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 照片1檔案名稱
        /// </summary>
        public string PIC1 { get; set; }

        /// <summary>
        /// 照片2檔案名稱
        /// </summary>
        public string PIC2 { get; set; }

        /// <summary>
        /// 照片3檔案名稱
        /// </summary>
        public string PIC3 { get; set; }

        /// <summary>
        /// 照片4檔案名稱
        /// </summary>
        public string PIC4 { get; set; }
    }
}